 private Dictionary<string, object> SetScreenLockPasswordApproveTime(Dictionary<string, object> param)
        {
            Dictionary<string, object> result = null;

            try
            {

                using (DataTable dt = dac.GetUserWithPassword(param))
                {
                    if (dt == null || dt.Rows.Count <= 0)
                    {
                        result = new Dictionary<string, object>();
                        result.Add("result", "failure");
                        result.Add("message", "계정정보를 확인해주세요.");
                        result.Add("code", "1");
                        return result;
                    }

                    string cipher_key = "NM_Cipher_@#$&&$(";
                    string password = Convert.ToString(param["password"]);
                    string encrypt_password = Convert.ToString(dt.Rows[0]["password"]);
                    string user_password = Decrypt(encrypt_password, cipher_key);
                    if (password != user_password)
                    {
                        result = new Dictionary<string, object>();
                        result.Add("result", "failure");
                        result.Add("message", "계정정보를 확인해주세요.");
                        result.Add("code", "1");
                        return result;
                    }

                    // admin 페이지에 로그인할 수 있는 레벨은 200이상부터.
                    int user_level = Convert.ToInt32(dt.Rows[0]["user_level"]);
                    if (user_level < 200)
                    {
                        result = new Dictionary<string, object>();
                        result.Add("result", "failure");
                        result.Add("message", "로그인 권한이 없습니다.");
                        result.Add("code", "1");
                        return result;
                    }

                    // 세션 로그인
                    UserInfo.GetInstance(Session).SignIn(dt.Rows[0]);

                    // 리턴값
                    result = new Dictionary<string, object>();
                    result.Add("result", "success");
                    result.Add("code", "0");
                    result.Add("company_no", dt.Rows[0]["company_no"]);
                    result.Add("user_id", dt.Rows[0]["user_id"]);
                    result.Add("user_no", dt.Rows[0]["user_no"]);
                    return result;
                }
            }
            catch (Exception e)
            {
                BasePage basePage = new BasePage();
                return basePage.GetErrorMessage(e, param);
            }
        }





