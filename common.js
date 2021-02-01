
//QR -- undefined --> return 
function ScanQRCodeCallback(qrcode) {
    if (g_ScanQRCode_callback == undefined)
        return;

    window[g_ScanQRCode_callback](qrcode);
}
// QR 내역을 handler 에 전달 
function ScanQRCodeCallback_SetScreenlock(qrcode) {
    //handler 에 있는 SetScreenlockQR 에 전달한다. 
    var param = 'cmd=SetScreenlockQR';
    param += '&qrcode=' + encodeURIComponent(qrcode);
    param += '&cmd_type=HTTP';
    param += '&callback=SetScreenlockQRCallback';
    NM.ExecuteCommand(param);
}

//QR 스캔 완료시 function() 
function SetScreenlockQRCallback(data) {
    var data = ConvertJson(data);
    if (data.code != 0)
        return;

    alert("스캔 완료. PC에서 확인 버튼을 눌러주세요.");
}
