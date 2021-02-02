   //tb_passwordform_password 참조
        //--> event function() --> Onclick button --> event 
        //passwordForm 에서 button click event 
        private void pb_passwordForm_approval_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_passwordform_password.Text.Trim()))
            {
                //Thread thread = new Thread(delegate ()
                //{
                //    MessageBox.Show(LanguageManager.GetString("screenForm_Msg_InputthePW"), LanguageManager.GetString("screenForm_Msg_Error"), MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                //});
                //thread.IsBackground = true;
                //thread.Start();
                return;
            }

            Dictionary<string, object> param = new Dictionary<string, object>();
            param["url"] = $"{ConfigurationManager.AppSettings["url"]}/handler.aspx";
            StringBuilder postData = new StringBuilder();
            postData.Append($"cmd=GetUserWithPassword");
            postData.Append($"&company_no={GlobalParam.GetInstance().userInfo.company_no}");
            postData.Append($"&user_no={GlobalParam.GetInstance().userInfo.user_no}");
            //--> input 내용을 가져와서 postData.Append 
            postData.Append($"&password={HttpUtility.UrlEncode(tb_passwordform_password.Text)}");
            param["postData"] = postData.ToString();
            string responseData = CommonHelper.RequestWebPage(param);
            if (string.IsNullOrEmpty(responseData))
            {
                //screenlock_password_approve_time --> 서버에 전달해야할 시간
                GlobalParam.GetInstance().agentContext.SetOfflineMode();
                GlobalParam.GetInstance().userInfo.screenlock_password_approve_time = GlobalParam.GetInstance().userInfo.current_time;
                GlobalParam.GetInstance().userInfo.isScreenlockPassword = false;
                this.Stop();
                return;
            }
            JObject responseJson = JObject.Parse(responseData);
            if (responseJson["code"].Value<int>() != 0)
            {
                tb_passwordform_password.Clear();

                Thread thread = new Thread(delegate ()
                {
                    MessageBox.Show(LanguageManager.GetString("screenForm_Msg_WrongPW"), LanguageManager.GetString("screenForm_Msg_FailedChange"), MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                });
                thread.IsBackground = true;
                thread.Start();
                return;
            }

            //screenlock_password 를 server(handler --> TKDacs (procedure) --> insert  
            //--> Default : password 
            //--> 이중보안락 --> password --> Qr code 
            // 정상적으로  이중보안락이 해제 될때 수행되는 구문
            // 서버로 승인시각을 보내주는 로직이 추가되어야함
            GlobalParam.GetInstance().userInfo.screenlock_password_approve_time = GlobalParam.GetInstance().userInfo.current_time;
            //GlobalParam.GetInstance().userInfo.screenlock_password_approve_time  를 handler 에 전달 
            param = new Dictionary<string, object>();
            param["url"] = $"{ConfigurationManager.AppSettings["url"]}/handler.aspx";
            postData = new StringBuilder();
            //cmd=SetScreenLockPasswordApproveTime --> cmd == handler fucntion()
            //Parameter로 handler 에 전달 --> user_no , company_no , SetScreenLockPasswordApproveTime 전달 
            postData.Append($"cmd=SetScreenLockPasswordApproveTime"); //전달할 handler function() 
            postData.Append($"&user_no={GlobalParam.GetInstance().userInfo.user_no}");//--> user_no
            postData.Append($"&company_no={GlobalParam.GetInstance().userInfo.company_no}");//-->company_no
            postData.Append($"&screenlock_password_approve_time={GlobalParam.GetInstance().userInfo.screenlock_password_approve_time}");
            param["postData"] = postData.ToString();
            responseData = CommonHelper.RequestWebPage(param);
            if (string.IsNullOrEmpty(responseData))
            {
                //null 
                GlobalParam.GetInstance().agentContext.SetOfflineMode();
                GlobalParam.GetInstance().userInfo.screenlock_password_approve_time = GlobalParam.GetInstance().userInfo.current_time;
                GlobalParam.GetInstance().userInfo.isScreenlockPassword = false;
                this.Stop();
                return;
            }    