/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Date:2024/12/25 9:13:59
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;

namespace UIFrameWork
{
	public class TempWindowDataComponent:MonoBehaviour
	{
		public Button CloseButton;

		public Toggle toggleToggle;

		public Image imgImage;

		public Image img14Image;

		public Image iwqeImage;

		public Button CloButton;

		public void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     TempWindow mWindow=(TempWindow)target;
		     target.AddButtonClickListener(CloseButton,mWindow.OnCloseButtonClick);
		     target.AddToggleClickListener(toggleToggle,mWindow.OntoggleToggleChange);
		     target.AddButtonClickListener(CloButton,mWindow.OnCloButtonClick);
		}
	}
}
