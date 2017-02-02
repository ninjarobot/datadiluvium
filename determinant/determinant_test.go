package determinant

import (
	"testing"
)

func TestAddTestsHere(t *testing.T) {
	result := "yes"
	expectedResult := "yes"

	if result != "yes" {
		t.Error(
			"For", result,
			"expected", expectedResult,
			"got", result,
		)
	}
}
