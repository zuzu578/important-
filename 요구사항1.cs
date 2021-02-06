1) 출근시간은 QR 코드를 입력 또는 이중보안의 비밀번호를 입력한 시간으로
전달 (Agent) --> 

* 궁극적인 요구사항 
==> QR 을 스캔한 시간을 DataBase 에 넣어주십쇼 


* 요구사항을 이행하기위한 선행지식 

1) QR system 이 작동하는 logic function()을 찾아야한다
2) QR system 이 작동하는 Database 를 찾아야한다
3) QR system 이 작동하는 procedure 를 찾아야한다
4) QR 스캔한 시간을 찾아야한다 ( 어디다 선언 했던것 을 찾아야한다 )
--> 원래 어딘가에서 사용하고 있는 내용을 찾아야한다



출근시간 ==> QR code 입력 
--> QR code ==> mobile app ==> QR scan ==> button(onClick)
--> QR 스캔 시간을 전달(DB) 
--> QR scan 에 대한 code --> common.js

* QR 에대한 page 따로 없음 --> common.js --> QR code scan
-> QR code 에 대한 parameter() handler(controller) 전달

--> default 
1) user_no
2) company_no
3) QR( QR approve time ) 
QR --> QR 을 스캔한 시간을 전달을 해줘야한다



요구사항 해결 과정 
1) QR Logic 에대한 function() 이있는 file 찾기 
=> TimeKeeper.Web / mobile / www / common.js 
=> 경로에서도 알다시피 , QR은 mobile 에서 스캔해야하는 기능이기때문에
Mobile node 에서 www / common.js 를 찾아가야한다

2) code에서 Qr 을 전달하는 Fucntion() 이 있었다
--> 이 parameter()를 어디에다 전달해주는지를 파악을 해줘야한다

3) 파악한 결과  var param = 'cmd=SetScreenlockQR'; 에 전달
되는 것이 보인다
Var param = "cmd = SetScreenlockQR"; --> handler function()

4) 결론적으로  QR parameter() 를 SetScreenlockQR이라는 function()
에 전달 해주고 있다 

5) handler.aspx.cs 라는 handler ( Controller class ) 에서 SetScreenlockQR function()을 찾
는다 
--> case 문에서 걸림 --> 그것을 추적해서 확인 
* procedure 에서도 company_no , user_no 를 parameter 로 전달 받기 때문에 
* handler 에서도 company_no , user_no 를 전달을 해주어야 하는 상황 
--> Qr안에 있는 parameter에 보이지는 않지만 data가 들어있데요 
그래서 추적을 해본결과 
Case 문에서 걸리게 되는데

즉 , SetScreenlockQR 여기에서 qrcode parameter() 를 전달을 받고 있는상황이네요 ? 

case "SetScreenlockQR":
                    {
                        //common.js 의 Qr function( ) 에서 전달된 parameter 값들을 받아낸다.
                        //QR --> qrcode , user_no , company_no 
                        Dictionary<string, object> param = new Dictionary<string, object>();
                        param.Add("screenlock_QR_no", Request.Params["qrcode"]);
                        param.Add("user_no", Request.Params["user_no"]);
                        param.Add("company_no", Request.Params["company_no"]);
                        //common.js 의 Qr function( ) 에서 전달된 parameter 값들을 Serialize 해준다

                        //QR code parameter()
                        Response.Write(new JavaScriptSerializer().Serialize(SetScreenlockQR(param)));
                        Response.End();
                    }
                    break;

// javascriptserializer() api : json , ajax 프로그램에서 사용할때 , 그 parameter() 전달하기 위한 api 



그래서 Response.Write(new JavaScriptSerializer().Serialize(SetScreenlockQR(param))); 
이부분을 추적해보니 




//--> DB 에 parameter()를 전달하기 위한 용도의 SetScreenlockQR
public Dictionary<string, object> SetScreenlockQR(Dictionary<string, object> param)
        {
            Dictionary<string, object> result = null;

            try
            {
                //qrcode --> param 
                string cipher_key = "NM_Cipher_@#$&&$(";
                string qrcode = Convert.ToString(param["qrcode"]);
                string screenlock_QR_info = $"{Decrypt(qrcode.Substring(3), cipher_key)}";
                string[] screenlock_QR_info_list = screenlock_QR_info.Split('_');
                if (screenlock_QR_info_list.Length != 3)
                {
                    result = new Dictionary<string, object>();
                    result.Add("result", "failure");
                    result.Add("code", "1");
                    return result;
                }

                // screenlock QR이 아닌 경우                
                if (screenlock_QR_info_list[0] != "SCREENLOCK")
                {
                    result = new Dictionary<string, object>();
                    result.Add("result", "failure");
                    result.Add("code", "2");
                    result.Add("message", "QRCode 를 확인해주세요.");
                    return result;
                }

                // 오늘 생성된 qrcode가 아닌 경우
                DateTime qrcode_time = Convert.ToDateTime(screenlock_QR_info_list[2]);
                //DateTime.Now.Date --> 오늘 날짜의 QR
                if (qrcode_time.Date != DateTime.Now.Date)
                {
                    result = new Dictionary<string, object>();
                    result.Add("result", "failure");
                    result.Add("code", "3");
                    result.Add("message", "최신 QRCode를 스캔해주세요.");
                    return result;
                }

                // 생성된 qrcode 의 사용자가 아닌 경우
                string user_no = Convert.ToString(screenlock_QR_info_list[1]);
                if (user_no != Convert.ToString(param["user_no"]))
                {
                    result = new Dictionary<string, object>();
                    result.Add("result", "failure");
                    result.Add("code", "4");
                    result.Add("message", "QRCode 사용자 정보가 다릅니다.");
                    return result;
                }

                // GPS로 출근 했는지 확인 : 20190826
                // 영업직 제외
                string user_work_type = string.Empty;

                param["user_no"] = screenlock_QR_info_list[1];
                using (DataTable dt = dac.GetUser(param))
                {
                    if (dt == null || dt.Rows.Count <= 0)
                    {
                        result = new Dictionary<string, object>();
                        result.Add("result", "failure");
                        result.Add("code", "5");
                        result.Add("message", "유저 정보가 없습니다.");
                        return result;
                    }
                    user_work_type = Convert.ToString(dt.Rows[0]["user_work_type"]);
                }

                if (user_work_type != "영업직")
                {
                    param["work_date"] = screenlock_QR_info_list[2];
                    using (DataTable dt = dac.GetWorkTimeUser(param))
                    {
                        if (dt == null || dt.Rows.Count <= 0)
                        {
                            result = new Dictionary<string, object>();
                            result.Add("result", "failure");
                            result.Add("code", "6");
                            result.Add("message", "출근 정보가 없습니다.");
                            return result;
                        }

                        string office_in_location_no = Convert.ToString(dt.Rows[0]["office_in_location_no"]);
                        if (office_in_location_no == "0")
                        {
                            result = new Dictionary<string, object>();
                            result.Add("result", "failure");
                            result.Add("code", "7");
                            result.Add("message", "출근 정보가 없습니다.");
                            return result;
                        }
                    }
                }
	       // 위의 문제되는 것이 없다 판단이 되면 
               // param 을 db 에 전달하게 됨 

                //출근시간은 QR코드를 입력하거나 이중보안의 비밀번호를 입력한 시간으로 전달.
                //--> QR 코드 입력 == QR code Scan 
                // QR code Scan parameter 를 DB 에 저장 
                // --> 위 의 모든 과정을 걸쳐서 Db에 저장 
                // DB 저장

                using (DataTable dt = dac.SetScreenlockQR(param))
                {
                    if (dt == null || dt.Rows.Count <= 0)
                    {
                        result = new Dictionary<string, object>();
                        result.Add("result", "failure");
                        result.Add("code", "7");
                        return result;
                    }
                    //qrcode --> param 
                    result = new Dictionary<string, object>();
                    result.Add("result", "success");
                    result.Add("code", "0");
                    result.Add("screenlock_QR_no", dt.Rows[0]["screenlock_QR_no"]);
                    return result;
                }
            }
            catch (Exception e)
            {
                BasePage basePage = new BasePage();
                return basePage.GetErrorMessage(e, param);
            }
        }


// 위에서 param == > Dictionary -- > user_no , company_no , screenlock_QR_no 의 3개 parameter() 를 전달하고있는 상황

public DataTable SetScreenlockQR(Dictionary<string, object> param)
        {
            // procedure 이름
            //--> QR code number , user_number , screenlock_approve_YN 
            string spName = "sp_screenlock_QR_insert";
            string connectionString = ConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
            List<SqlParameter> sqlParam = new List<SqlParameter>();
            sqlParam.Add(new SqlParameter("@screenlock_QR_no", m_dbHelper.GetValueFromParam(param, "screenlock_QR_no")));
            sqlParam.Add(new SqlParameter("@user_no", m_dbHelper.GetValueFromParam(param, "user_no")));
            sqlParam.Add(new SqlParameter("@screenlock_approve_YN", m_dbHelper.GetValueFromParam(param, "screenlock_approve_YN")));
            return m_dbHelper.ExecuteSP(spName, connectionString, sqlParam);
        }


이제 프로시저를 보도록 하자 

CREATE PROCEDURE [dbo].[sp_screenlock_QR_insert]		
	@screenlock_QR_no			INT,
	@user_no					INT,
	@screenlock_approve_YN		CHAR(1)	
AS
BEGIN	
	SET NOCOUNT ON;


	-- 만약 handler 에서 전달 해준 parameter 들 중에서 screenlock_QR_no 에 해당하는 paramter()가 null 값 일 경우에 
	IF (@screenlock_QR_no IS NULL)
	BEGIN	
		INSERT INTO [dbo].[tb_screenlock_QR] WITH(ROWLOCK)
		(user_no)
		VALUES
		(@user_no)

		SELECT SCOPE_IDENTITY() AS screenlock_QR_no
	END
	ELSE
	-- screenlock_QR_no 에 해당하는 parameter() 가 null로 넘어오지 않은 경우 (정상적으로 값이 전달된경우) 

	-- 이 BEGIN부터 시작하는 procedure 의 의미는 무엇이냐면 
	--1) UPDATE [dbo] . [tb_screenlock_QR]테이블을 update --> user_no가 null값일때는 추가해주고 , 또 ,screenlock_approve 가 null
         -- 일때 추가 , 아니면 , approve_YN = y 로 값을 설정을해주고( 요구사항에서 QR스캔한 --> 출근으로 인정 , )
	-- 정상적으로 paramter() 가 전달이 되었으면 (QR이 잘스캔된거겟죠 ) --> approve_YN = Y , 그리고 ,SYSDATETIME() ==> 그 출근한 시간을
        -- 자동으로 찍게 해주는 ( Y , 현재 출근시간으로 DATABASE 에 저장하도록 하게 설계를 하는 것 ). 


	BEGIN		
		UPDATE [dbo].[tb_screenlock_QR] WITH(ROWLOCK)
		-- 여기서도 , 만약 handler 에서 전달받은 user_no 가 null 값으로 전달 된 경우일때 
		SET user_no = CASE WHEN @user_no IS NULL THEN user_no ELSE @user_no END
		,screenlock_approve_YN = CASE WHEN @screenlock_approve_YN IS NULL THEN screenlock_approve_YN ELSE @screenlock_approve_YN END
		,screenlock_approve_time = CASE WHEN @screenlock_approve_YN = 'Y' THEN SYSDATETIME() ELSE screenlock_approve_time END
		, updatedTime = SYSDATETIME()
		WHERE screenlock_QR_no = @screenlock_QR_no		
		
		SELECT @screenlock_QR_no AS screenlock_QR_no
	
		
	-- 당일 정보가 없는 경우 정보 insert
	IF NOT EXISTS(
		SELECT *
		FROM [dbo].[tb_work_time_user] WITH(NOLOCK)
		WHERE user_no = @user_no
		AND work_date = CONVERT(DATE, GETDATE())
	)
	BEGIN
		EXEC [dbo].[sp_work_time_user_init] @user_no
	END

	UPDATE [dbo].[tb_work_time_user] WITH(ROWLOCK)
	SET office_in_time = CASE WHEN office_in_time IS NULL THEN SYSDATETIME() ELSE  office_in_time END
	WHERE user_no = @user_no
	AND work_date = CONVERT(DATE, GETDATE())

	END

END






        



