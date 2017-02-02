package determinant

import (
	"encoding/json"
	"fmt"
	"os"
)

type Page struct {
	Title string `json:"title"`
	Schema   string `json:"schema"`
}

func (p Page) toString() string {
	return toJson(p)
}

func toJson(p interface{}) string {
	bytes, err := json.Marshal(p)
	if err != nil {
		fmt.Println(err.Error())
		os.Exit(1)
	}

	return string(bytes)
}
