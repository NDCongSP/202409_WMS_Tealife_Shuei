//focus vao element textbox và xóa data
function FocusElementText(objId) {

    if (document.getElementById(objId) == null) return;

    var _element = document.getElementById(objId);

    if (objId != "_dropDownSelectBox" && objId != "_btnComplete")
        _element.value = "";

    _element.focus();
}

//Call API at client for print
function callApiPost(printerName, printData) {
    $.ajax({
        url: "http://localhost:4000/PrintClient/Print",
        method: "POST",
        crossDomain: true,
        contentType: "application/json",
        data: JSON.stringify({
            PrinterName: printerName,
            PrintData: printData
        }),
        success: function (response) {
            console.log("Data posted successfully:", response);
            //alert("Print packing label successfully!");
        },
        error: function (xhr, status, error) {
            console.error("Error occurred:", error);
        }
    });
}

function ClosePopupDropDownGrid(objId,isVisible) {
    var popup = document.getElementById(objId); // Thay đổi selector nếu cần
    if (popup) {
        //var popupId = popup.id;
        //if (popupId) {
        //    Radzen.closePopup(popupId);
        //}

        popup.style.visibility = isVisible;
    }
}

function downloadFileFromBytes(data, fileName, contentType) {
    // Create a blob from the byte array
    const blob = new Blob([data], { type: contentType });
    
    // Create a temporary URL for the blob
    const url = window.URL.createObjectURL(blob);
    
    // Create a temporary link element
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    
    // Append link to body, click it, and remove it
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    // Release the URL object
    window.URL.revokeObjectURL(url);
}
