package filters

import (
	"crypto/md5"
	"encoding/hex"
	"fmt"
	"github.com/gin-gonic/gin"
	"net/http"
	"net/url"
	"sort"
	"strconv"
	"strings"
	"time"
)

// var ACCESS_KEY string
// var SECRET_KEY string
const (
	//请求延时时间，分钟  默认2分钟
	delayMinits float64 = 2.0
)

// var SignKeys=make(map[string]string)
var SignKeys = map[string]string{
	"6800168364": "Fr2rsAJYtqlolNZVwNuTqMoBU8sFCdMF",
	"5135911898": "d2a57dc1d883fd21fb9951699df71cc7",
	"3175298382": "7e02e9c39dc448dc901ace3229426a37",
}

func ApiSignMIddleware() gin.HandlerFunc {
	return func(c *gin.Context) {

		/*
			// 获取请求头Header中的 key sign timestamp
			token := c.Request.Header.Get("X-Auth-Token")
			key := c.Request.Header.Get("X-Auth-Key")
			timestamp := c.Request.Header.Get("X-Auth-TimeStamp")
			fmt.Printf("[info] key is %s ,timestamp is %s\n", key, timestamp)
			//获取所有url请求参数
			reqData := c.Request.URL.Query()
			fmt.Printf("[info] req url data is %s\n", reqData)

			// //获取post body
			c.Request.ParseForm()
			reqBodyData := c.Request.PostForm
			fmt.Printf("[info] req body data is %s \n", reqBodyData)

			//获取Post put请求模式下的Content-length
			conlength := c.Request.Header.Get("Content-Length")
			fmt.Printf("[info] Content-Length is %s\n", conlength)

			//判断请求Method
			// GET POST PUT DELETE  OPTIONS
			fmt.Println("[info]", c.Request.Method)
			fmt.Println("[info]", c.Request.Host) // localhost:8080
			fmt.Println("[info]", c.Request.URL)  ///?id=3&name=zhangsan&address=beijing
			fmt.Println("[info]", c.Request.ContentLength) // 16  or 0

			if token == "" {
				c.Abort()
				return
			}
		*/

		// t_time := time.Now().Unix()
		// fmt.Println(t_time)
		// //将时间戳格式化显示为日期
		// t_m := time.Unix(t_time, 0)
		// fmt.Println(t_m)
		// fmt.Println(t_m.Format("2006-01-02 15:04:05"))

		//计算时间差值
		// span := dtTimeSpan("a")
		// fmt.Println(span)

		/*
			验证流程
			1. 判断三个必需参数是否存在
			2. 验证是否超时
			3. 判断key是否存在
			4. 验证Sign是否正确

		*/

		c_sign := c.Request.Header.Get("X-Auth-Sign")
		c_key := c.Request.Header.Get("X-Auth-Key")
		c_timestamp := c.Request.Header.Get("X-Auth-TimeStamp")

		//判断请求头中是否含有必须的三个必需参数
		if c_sign == "" || c_key == "" || c_timestamp == "" {
			c.JSON(403, ReturnMsg{Code: 1, Msg: "Error Request parameters"})
			c.Abort()
			return
		}

		//验证是否超时
		// c_deM := dtTimeSpanMinutes(c_timestamp)
		// if c_deM > delayMinits {
		// 	c.JSON(403, ReturnMsg{Code: 1, Msg: "Request Timeout"})
		// 	c.Abort()
		// 	return
		// }

		//判断key是否存在，并获取对应的secret值
		c_key_value, c_ok := SignKeys[c_key]
		if !c_ok {
			c.JSON(403, ReturnMsg{Code: 1, Msg: "Key Error"})
			c.Abort()
			return
		}
		// fmt.Println(c_key_value)

		//获取请求Method
		// c_method:=c.Request.Method

		//验证Sign是否正确
		isOK := VerifySign(c.Request, c_key, c_key_value, c_timestamp, c_sign)
		if !isOK {
			c.JSON(403, ReturnMsg{Code: 1, Msg: "Sign Error"})
			c.Abort()
			return
		}

		fmt.Println("[info] start next")
		c.Next()
		fmt.Println("[info] end next")
	}
}

type ReturnMsg struct {
	Code int         `json:"code"`
	Msg  string      `json:"msg"`
	Data interface{} `json:"data"`
}

//计算一个字符时间戳与当前时间的时间差（分钟）
func dtTimeSpanMinutes(unix string) float64 {
	//将字符串转换为相应类型的数字
	i, err := strconv.ParseInt(unix, 10, 64)
	fmt.Println(i, err)
	if err != nil {
		i = time.Now().AddDate(0, 0, -7).Unix()
	}
	//将Unix时间戳转换为time类型
	tm := time.Unix(i, 0)
	tm_now := time.Now()
	//计算两个时间差
	tm_span := tm_now.Sub(tm)
	return tm_span.Seconds() / 60.0
}

//验证sign是否正确
func VerifySign(r *http.Request, keyValue string, signValue string, timeStamp string, urlSign string) bool {
	// fmt.Println(r.Method)
	// fmt.Println(r.ContentLength)
	// fmt.Println(r.URL)
	// fmt.Println(r.URL.Query())
	// fmt.Println(signValue, timeStamp)

	v_method := r.Method
	v_contentlength := strconv.FormatInt(r.ContentLength, 10)
	v_url := r.URL.String()
	v_query := r.URL.Query() //获取到的值为数组形式

	//将所有参与sign验证参数保存到map(sign不参与签名计算)
	m_sign := map[string]string{
		"key":           keyValue,
		"method":        v_method,
		"contentlength": v_contentlength,
		"uri":           v_url,
		"timestamp":     timeStamp,
	}

	//	当为get或delete请求时，url中的参数参与签名计算
	if v_method == "GET" || v_method == "DELETE" {
		for k, v := range v_query {
			// fmt.Println("[info]-", k, v)
			//排除键为空或值为空
			if k == "" || v[0] == "" {
			} else {
				m_sign[k] = v[0]
			}
		}
	}
	// fmt.Println("[info] ", m_sign)
	//按字母序排序并拼接成键值对形式
	sPara := filterParams(m_sign)
	fmt.Println(sPara)

	//获得签名结果
	mySign := createSign(sPara, signValue)
	fmt.Println("urlSign:", urlSign, "mySign:", mySign)
	if mySign == urlSign {
		return true
	} else {
		return false
	}
}

//将map以字母a到z的顺序排序,按照“参数=参数值”的模式用“&”字符拼接成字符串
func filterParams(m map[string]string) (r_s string) {
	var keys []string
	for k := range m {
		// fmt.Println(k)
		keys = append(keys, k)
	}
	//升序排列
	sort.Strings(keys)
	// fmt.Println(keys)
	//
	for _, k := range keys {
		// fmt.Println("key:", k, "Value :", m[k])
		// 对请求参数中的中文进行编码处理
		r_s += k + "=" + url.QueryEscape(m[k]) + "&"
	}

	//最末尾的 & 不需要去除,后面还要拼接secret
	// //去掉最後一個&字符
	// if strings.HasSuffix(r_s, "&") {
	// 	r_s = strings.TrimRight(r_s, "&")
	// }
	return
}

//生成签名字符串
func createSign(prestr string, secret string) (mysign string) {
	//拼接 sign_str=md5(prestr+"secret="+md5(secret))
	prestr = prestr + "secret=" + GetMd5Hash(secret)
	// fmt.Println(prestr)
	mysign = GetMd5Hash(prestr)
	return
}

//获取字符串的MD5值
func GetMd5Hash(text string) string {
	hasher := md5.New()
	hasher.Write([]byte(text))
	md5_str := hex.EncodeToString(hasher.Sum(nil))
	return strings.ToUpper(md5_str)
}
