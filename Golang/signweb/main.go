package main

import (
	"fmt"
	"github.com/gin-gonic/gin"
	"signweb/filters"
)

func main() {
	router := gin.Default()

	api := router.Group("/api", filters.ApiSignMIddleware())
	v1 := api.Group("/v1")
	{
		fmt.Println("[Info] line 15")
		v1.GET("/user", getUserInfo)
		v1.POST("/add", postUserInfo)
	}

	v2 := api.Group("/v2")
	{
		v2.GET("/user", getUserV2)
		v2.POST("/user", postUserV2)
		v2.PUT("/user", putUserV2)
		v2.DELETE("/user", deleteUserV2)
	}

	v3 := api.Group("/v3")
	{
		v3.GET("/student", getStudentV2)
		v3.POST("/student", postStudentV2)
	}

	router.Run(":8080")
}

func getStudentV2(c *gin.Context) {
	uname := c.Query("name")
	c.JSON(200, gin.H{
		"stuName": uname,
	})
}

func postStudentV2(c *gin.Context) {
	var stu User
	c.BindJSON(&stu)
	c.JSON(200, gin.H{
		"stuName": stu.Name,
	})
}

func getUserV2(c *gin.Context) {
	c.String(200, "Hello get v2")
}

func postUserV2(c *gin.Context) {
	c.String(200, "Hello post v2")
}

func putUserV2(c *gin.Context) {
	c.String(200, "Hello put v2")
}

func deleteUserV2(c *gin.Context) {
	pid := c.Query("id")
	c.String(200, pid)
}

func getUserInfo(c *gin.Context) {
	c.String(200, "Hello LiMing")
}

func postUserInfo(c *gin.Context) {

	var json User
	c.BindJSON(&json)
	fmt.Println(c.ContentType())
	c.JSON(200, gin.H{
		"status":  "ok",
		"message": json.Name,
	})
}

type User struct {
	Name    string `json:"name"`
	Address string `json:"address"`
}
