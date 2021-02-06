2) 준비시간 이전에 QR코드 또는 , 비밀번호 입력이 가능하게 하기 , 입력한 시간을 전달 , 단 잠금화면은 준비시간 시작 이후에 
해제 되어야한다 .

==> timekeeper : * 준비시간 
준비시간 : 보통 근로시간 시작 하기 전의 준비시간을 말하는건데 
근로시간 이전에도 사용자가 준비작업을 할수 도있으니 , 잠그는거 말고 , QR 코드라던가 , 비밀번호 입력을 하게 해서 해제 하도록 하게 해주세요 

그렇다면 
QR 코드를 창에 띄우게 할것이냐 , 아니면 비밀번호를 입력하게 할것이냐를 설정할수있는 방법은 
Timekeeper web 에서 설정이 가능 

Timekeeper Agent 에 대한 설명 

궁극적으로 이 부분은 
Timekeeper Agent 의 screenLock.cs 에 이러한 내용이 있을 거에요 

ScreenLock.cs 에서 이부분이 있는데 

이부분이 뭐냐면 
Timekeeper web에서 qr을 띄울거냐 아니면 password 창을 띄울거냐 설정을 할수있는데 ( 이중 보안 설정 ) 

만약에 qr 을 띄우게 하는 설정을 했으면 
screenlock_qr 에 걸리고 , 아니면 password 에 걸리는데 


==> 우선순위가 password 가 우선순위( default ) 그다음이 qrcode 로 하게 끔하는 설정으로 되어있습니다 .
그래서 우리가 집중적으로 봐야할 부분은 screenlock_password 이부분이 되는것이죠 

 
else if (type == "screenlock_QR")
                {
                    //--> QR screen 
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            HideAllControls();
                            qrcodepanel.Visible = true;
                            bt_qrcode_accept.Visible = true;
                            bt_force_use.Location = new Point(12, 12);
                            bt_force_use.Visible = true;
                            bt_screenlock_refresh.Location = new Point(156, 12);
                            bt_screenlock_refresh.Visible = true;
                            GetQrImage();//--> QR image 가져옴 
                            lb_qrcode_info.Text = LanguageManager.GetString("screenForm_Msg_MobilePushtheButton");
                        }));
                    }
                    else
                    {
                        HideAllControls();
                        qrcodepanel.Visible = true;
                        bt_qrcode_accept.Visible = true;
                        bt_force_use.Location = new Point(12, 12);
                        bt_force_use.Visible = true;
                        bt_screenlock_refresh.Location = new Point(156, 12);
                        bt_screenlock_refresh.Visible = true;
                        GetQrImage();
                        lb_qrcode_info.Text = LanguageManager.GetString("screenForm_Msg_MobilePushtheButton");
                    }
                    return true;
                }
                //=> web --> screenlock --> screenlock_password 
                else if (type == "screenlock_password")
                {
                    //--> screenlock_password 
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            HideAllControls();
                            bt_screenlock_refresh.Location = new Point(12, 12);
                            bt_screenlock_refresh.Visible = true;
                            bt_force_use.Location = new Point(156, 12);
                            bt_force_use.Visible = true;
                            lb_passwordfrom_user.Text = GlobalParam.GetInstance().userInfo.name;
                            tb_passwordform_password.Clear();
                            passwordPanel.Visible = true;//--> screenlock_password 보임
                        }));
                    }
                    else
                    {
                        HideAllControls();
                        bt_screenlock_refresh.Location = new Point(12, 12);
                        bt_screenlock_refresh.Visible = true;
                        bt_force_use.Location = new Point(156, 12);
                        bt_force_use.Visible = true;
                        lb_passwordfrom_user.Text = GlobalParam.GetInstance().userInfo.name;
                        tb_passwordform_password.Clear();
                        passwordPanel.Visible = true;
                    }
                    return true;
                }


