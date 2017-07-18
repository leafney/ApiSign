### 接口签名验证流程


#### 分配Secret

为指定应用分配接口调用的 `AppKey` 和 `AppSecret`

#### 接口签名规则

##### sign接口标准

1. 为保证接口通信安全，需要对数据通信进行签名和验证。
2. 数据通信使用 `AppKey(应用ID)` 和 `AppSecret(应用密钥)` 来进行验证，由服务商提供。
3. 统一采用UTF-8字符编码
4. 签名算法默认为 `MD5` ，后续会兼容 `SHA1` 、`SHA256`、`HMAC` 等

##### 接口调用规则

1. 请求采用 `restful` 标准即 `GET` `POST` `PUT` `DELETE` 方式，字符集使用UTF-8。
2. `AppKey` 是合作伙伴身份的唯一标识，加密密钥 `AppSecret` 用来对提交的数据和通知的数据进行签名。
`AppKey` 会在网络上进行传输，而 `AppSecret` 不会在网络上进行传输。
3. 所有请求都需要在请求头 `Header` 中附加上 `X-Auth-Key`、`X-Auth-Sign`、`X-Auth-TimeStamp`三个字段，区分大小写。`X-Auth-Key`为唯一标识 `AppKey` ; `X-Auth-Sign`为签名; `X-Auth-TimeStamp` 为 `10` 位长度的unix时间戳。
4. 默认返回数据格式为`json`格式。
5. 接口请求方式 `METHOD` 参与sign签名验证，请求键名称为 `method`，值为请求类型的大写形式如 `GET`、`POST`、`PUT`、`HEAD` 等
6. 接口请求路径 `URI` 参与sign签名验证，请求键名称为  `uri` ，必须符合 `http` 协议标准：包含中文名称或特殊字符的文件名（或目录），需进行 `urlencode` 处理
7. 请求内容长度 `CONTENT_LENGTH` 参与sign签名验证，需要与request header 中的 `Content-­Length` 一致。若是 `GET` `DELETE` 方法，该参数值为 `0`
    * GET DELETE 等，所有参数都需要进行签名运算，Content-Length长度为0
    * POST PUT 等，请求参数不参加签名运算，Content-Length长度为请求内容长度


5. 在 `Header` 中传递 `X-Auth-Key`、`X-Auth-Sign`、`X-Auth-TimeStamp` 三个关键验证字符串:   
    * 如果是 `GET` `DELETE` 请求，请求参数一般在 `url` 中传递，则需要将所有请求参数都参与进行sign签名处理(注意：**如果参数的值为空不参与签名**)
    * 如果是 `POST` `PUT` 请求，提交数据一般在 `body` 中以 `json` 格式传递，则签名操作仅对这四个参数进行签名处理即可


如果参数的值为空不参与签名；
◆ 参数名区分大小写；
◆ 验证调用返回或微信主动通知签名时，传送的sign参数不参与签名，将生成的签名与该sign值作校验。

参与签名计算的必需参数：(必需参数的键名均为小写形式，键值区分大小写)

| 参数名 | 参数值 | 说明 |
| ----- | ----- | ----- |
| `key` |   |   |
| `method` |     |    |
| `uri` |    |    |
| `contentlength` |   |   |
| `timestamp` |    |     |


#### sign签名方法

1. 筛选
获取所有必需参数，请求参数不包括字节类型参数，如文件、字节流，不包括值为空的参数，剔除sign字段。
2. 排序  
将筛选的参数按照第一个字符的键值ASCII码递增排序（字母升序排序），如果遇到相同字符则按照第二个字符的键值ASCII码递增排序，以此类推。
3. 拼接  
将排序后的参数与其对应值，组合成“参数=参数值”的格式，并且把这些参数用&字符连接起来，此时生成的字符串为待签名字符串string1。
	* 例：假如参数有id=2108 name='hello' ,则 `string1 = id=2108&key=210000001&name=hello&timestamp=1234567890 `
4. 加密钥  
在 string1 最后拼接上 `secret=appsecret(商户密钥)` 得到 stringSignTemp
	* 例：`string1 = id=2108&key=210000001&name=hello&timestamp=1234567890&secret=3747jfudjfejwo837dj4d7 `

5. MD5  
对stringSingTemp进行MD5算法生成sign，再将得到的加密字符串转换为大写。即得到签名sign。

6. 调用  
最后的调用格式如下：  
```
http://api.test.com/getproducts?key=2088911242&sign=07BED585D813370209B20A8B9F2C1AFB&timestamp=1460602476&abc=hello&pageindex=1&pagesize=10&style=nor&参数1=value1&参数2=value2.......
```

*** 

#### 服务端如何验证sign

1. 用户验证，判断key是否存在，同时查询出对应的secret用于验证签名
2. 验证时间戳，判断请求是否过期
3. 验证签名，根据算法将参数进行签名得到sign与参数中的sign进行对比
4. 根据请求参数进行对应的操作
5. 返回结果


***

#### 相关参考

* [微信支付接口api](https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=4_3)



