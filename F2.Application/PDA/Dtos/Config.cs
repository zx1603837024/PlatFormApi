using System;

namespace F2.Application.PDA.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class Config
    {
		public Config()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		// 应用ID,您的APPID
		public static string AppId = "2018020602152851";

		// 支付宝网关
		public static string Gatewayurl = "https://openapi.alipay.com/gateway.do";

		// 商户私钥，您的原始格式RSA私钥
		public static string PrivateKey = "MIIEpQIBAAKCAQEApOJ0Z/Wr5TBL7bkyxCM/V9Uvh9z+TxZ5wivz7Gtcdhk8oUgPCqR6l7SgH8B6t9Xpx28QkcDn4QmScON09bKCTH2eiovqowwHP5pVIRq6FyZX6vqd3DAe4bxgiwfZdoZFzPOc5Iggt2hx1WNswLlNj3RkqLlvMRC7j83KM6zcLuvUsJdN+9J4M035HSlhEVR4ETIg2l/Uw8ZRBAicC/zcGXMhaW3GteF1M0XDae1IxayYOZcFhBrtykZjkki036IrWVKAjXG8u2Oe60xVeqeXDhhcmi/jJyZbbzPvf8i3+NbyF55+eOmzgThVskXOSOUBDqXjDRAkLkoNmjYahy2o1wIDAQABAoIBACVWpte3+YJAKqGd09I4zSpdu5K/x9MvGsmO3IEIWvrfgixfvhBB6QUbwTsPgnrI9VTVYOIw1hHO8hugVqchEoKx28oyHyNnIOkVUPvbKpL9euFaeY8YUpX5677wOx2tEHd2H2xdK1N9UnHpPwJxtA+tYvIWdAifamoJRyXmmkIBhjBcZAZU9f0Lm2okAJn4/8j1ajyrCV8j+NWbGyqGKY0sOlVsvwNp/HB04yTpdYeVqhNa7D2D7JkXyvJISeW5br/b7JjSHcoC6l3KTYPexVt5UnMqD3oUGrNx+EKhwxRlLXpS/5F+KiAcBP+hZblyv9E4CZHdSg7ywVVqcr40OZkCgYEA47XILy7BdaGfnVQcFhPtHrjlVRjxCksl3WqKj4WLd0Q7Y8+02tJ3zLhtuZEeYnDRlbv4IvE2hfT1+LoLEubuiG/AEVIUskYKAjywkokxKocuPdQJFGo9NsnpRW9D45ydjOwgUo+Od0kImcgusgF4ZSVehc4PQlrJWV2P9gxExo0CgYEAuV6JJk8ybx7BK3lM3KC/CE60r7TaaPB82Vgvzx+PqlwhReUVhslDMboBh8n/ajzbq8tY0xOAcVZirxUcN6DGdvSic5DfzINhBdcU3XPmAdE7IA8xyNGm39PwpOnuazRbLAN7p6sXGzL96mQAxCdmp0MQopurlapdqlmY8aJyNfMCgYEA0OVA4lgITjuZTCGPKonZLCf+6xRYfFL4R6mnt4aRrTZBLY2soloWYQ6ISoZg03ogKzcnqhaTKEzTIfy6j/qetTefgcRFDE7Ie8jlS2jkW9lriHjuY3Ya9hpBD9vE8hgJSPCNrm/YCIJNi5ZjkBVdN50Sm7mvsfXLZVo56UTUCzkCgYEAoirYJVGnt5raN6Q439MpDKV+YTEA2kl5j9MckED2OH29Bh6xxGcSh9BIQnkvH8v6CcMuBArNlVmTn8QCGZBnz+2YqS2W/J37JVbShCI+mFxpM1TXou325V3O6xdUYyk8kbbCWR7OahckGAhAu1oU7kM3rGovpBGVO9CCgvIwpI0CgYEAl+hwSxJsJSv0K3KCzD3jA6eG0Tjr/YS2RRDcIlVMeRnLla4Mnz3xVVQhjauwbEf7zqQzSGO04DPjif4vYD5c7uaQ7zALI5eQVUTLEUabZUDZfjk1wo6G+esk5l4v4mKpvR6LxgR1c3Ms3kfpJg1rACrIUB61xuD+8rknVQ8e8q8=";


		// 支付宝公钥,查看地址：https://openhome.alipay.com/platform/keyManage.htm 对应APPID下的支付宝公钥。
		public static string AlipayPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAm6GWdxvnFGgVlE88k1ujC83qgZgiDUyX7hmuI4ApI4OgMtcDU+tcmCH3CQ4FvH2dDvRVm6F9a+8pX2mZYz/sG4js2qu2oYbVZsnAJ1O4sammZVGOpm651QgB4AIRoUywxcTaahsvhbT0AUZpLWQNl/Ua2bfKyNcFafSRjOlzzYF4VwNZqHAIypMnCv39lYel+PzA/m0BYPawsF0u2R0N95TvHinwx4MEVemzYoR+aeJcICg8VEerYUgfCsc8G2bIN268aiiM5bwzH0fg6ksJIkCe/xFPNrCUVOJuMkLJnHZXQ08S+HcIUKkbiUK3fFT8yfzjoOQp45mXt9Wdon3qaQIDAQAB";

		// 签名方式
		public static string SignType = "RSA2";

		// 编码格式
		public static string CharSet = "UTF-8";

        
    }
}
