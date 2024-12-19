/*------------------------
 *Title:UI自动化组件查找代生成工具
 *Date:2024/12/19 9:06:04
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体一键生成UI组件查找脚本即可
 *注意：以下文件是自动生成的，任何手动修改都会被下次生成覆盖，若手动修改后，尽量避免自动生成
------------------------*/
using UnityEngine.UI;
using UnityEngine;

namespace UIFrameWork
{
	public class TempWindowUIComponent
	{
		public  Button CloseButton;

		public  void InitComponent(WindowBase target)
		{
		     //组件查找
		     CloseButton =target.transform.Find("UIContent/[Button]Close").GetComponent<Button>();
	
	
		     //组件事件绑定
		     TempWindow mWindow=(TempWindow)target;
		     target.AddButtonClickListener(CloseButton,mWindow.OnCloseButtonClick);
		}
	}
}
