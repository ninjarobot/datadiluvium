package main

import (
	"testing"
)

func TestRdbmsConfiguration(t *testing.T) {
	schemaRdbms := getPages()[0]
	expectedResult := "rdbms"

	if schemaRdbms.Schema != expectedResult {
		t.Error(
			"For", schemaRdbms.Schema,
			"expected", expectedResult,
			"got", schemaRdbms.Schema,
		)
	}
}

func TestKeyValueConfiguration(t *testing.T) {
	schemaRdbms := getPages()[1]
	expectedResult := "kv"

	if schemaRdbms.Schema != expectedResult {
		t.Error(
			"For", schemaRdbms.Schema,
			"expected", expectedResult,
			"got", schemaRdbms.Schema,
		)
	}
}

func TestTimeSeriesConfiguration(t *testing.T) {
	schemaRdbms := getPages()[2]
	expectedResult := "ts"

	if schemaRdbms.Schema != expectedResult {
		t.Error(
			"For", schemaRdbms.Schema,
			"expected", expectedResult,
			"got", schemaRdbms.Schema,
		)
	}
}

func TestGeoDataConfiguration(t *testing.T) {
	schemaRdbms := getPages()[3]
	expectedResult := "geo"

	if schemaRdbms.Schema != expectedResult {
		t.Error(
			"For", schemaRdbms.Schema,
			"expected", expectedResult,
			"got", schemaRdbms.Schema,
		)
	}
}
