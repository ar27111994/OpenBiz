﻿// checksum calculation for GTIN-8, GTIN-12, GTIN-13, GTIN-14, and SSCC
// based on http://www.gs1.org/barcodes/support/check_digit_calculator
//Credit Steve Spigarelli https://stevespiga.rel.li
function isValidBarcode(barcode) {
  // check length
  if (barcode.length < 8 || barcode.length > 18 ||
      (barcode.length != 8 && barcode.length != 13 &&
      barcode.length != 14 && barcode.length != 18)) {
    return false;
  }

  var lastDigit = Number(barcode.substring(barcode.length - 1));
  var checkSum = 0;
  if (isNaN(lastDigit)) { return false; } // not a valid upc/ean

  var arr = barcode.substring(0, barcode.length - 1).split("").reverse();
  var oddTotal = 0, evenTotal = 0;

  for (var i = 0; i < arr.length; i++) {
    if (isNaN(arr[i])) { return false; } // can't be a valid upc/ean we're checking for

    if (i % 2 == 0) { oddTotal += Number(arr[i]) * 3; }
    else { evenTotal += Number(arr[i]); }
  }
  checkSum = 10 - ((evenTotal + oddTotal) % 10);

  // true if they are equal
  return checkSum == lastDigit;
}