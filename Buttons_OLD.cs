using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRLoader.Attributes;
using VRLoader.Modules;
using VRCSDK2;
using UnityEngine.Events;
using VRC.UI;

namespace Ruby.ButtonAPI
{
    [ModuleInfo("ButtonAPI", "2.0", "DubyaDude")]
    public class ButtonAPI : VRModule
    {
        public static QMSingleButton Q_RUBYLogo;
        public static QMNestedButton Q_Functions;
        public static QMToggleButton Q_F_Gain;

        public IEnumerator Start()
        {
            //Q_RUBYLogo is initially set as a QMSingleButton, so we want to instantiate 
            //a new one with all of the properties that we want
            Q_RUBYLogo = new QMSingleButton(
               //BtnMenu -> this has 3 possible types, "ShortcutMenu", "UserInteractMenu" or a QMNestedButton (button that opens up to other buttons)
               //ShortcutMenu -> main quick menu with worlds/avatars
               //UserInteractMenu -> quick menu when selecting a player
               "ShortcutMenu",
               //The x and y coordinates of the button. 0,0 is considered one to the left of worlds button.
               //x -> positive moves you to the right
               //y -> positive moves you down
               5, -1,
               //The main text of the button
               "RUBY",
               //The action you want the button to do when it is pressed, delegate() { } is a must have
               delegate()
               {
                   System.Diagnostics.Process.Start("https://www.vrchaven.com/");
                   QuickMenuStuff.GetQuickMenuInstance().CloseMenu();
               },
               //The tooltip of the button (the text on top when hovering over the button)
               "Client by DubyaDude <3\nClicking here will open a browser to the website.",
               //First color is background color, second color is text color. If not filled in or set to null, default colors will stay
               Color.cyan, Color.white
           );
            //Now you have a new button!



            //This creates a custom sprite for the Q_RUBYLogo button
            Sprite logoSprite = new Sprite();

            using (WWW www = new WWW("https://www.vrchaven.com/img/logo.png"))
            {
                yield return www;

                logoSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height),
                    new Vector2(0, 0));
            }

            Q_RUBYLogo.getGameObject().GetComponentInChildren<UnityEngine.UI.Image>().sprite = logoSprite;
            Material logoMaterial = new Material(Q_RUBYLogo.getGameObject().GetComponentInChildren<UnityEngine.UI.Image>().material);
            logoMaterial.shader = Shader.Find("Unlit/Transparent");
            Q_RUBYLogo.getGameObject().GetComponentInChildren<UnityEngine.UI.Image>().material = logoMaterial;


            //Q_Functions is initially set as a QMNestedButton, so we want to instantiate 
            //a new one with all of the properties that we want
            Q_Functions = new QMNestedButton(
                //BtnMenu -> this has 3 possible types, "ShortcutMenu", "UserInteractMenu" or a QMNestedButton (button that opens up to other buttons)
                //ShortcutMenu -> main quick menu with worlds/avatars
                //UserInteractMenu -> quick menu when selecting a player
                "ShortcutMenu",
                //The x and y coordinates of the button. 0,0 is considered one to the left of worlds button.
                //x -> positive moves you to the right
                //y -> positive moves you down
                5, 2,
                //The main text of the nested button
                "Functions",
                //The tooltip of the button (the text on top when hovering over the button)
                "A little bit of extra stuff",
                //(Nested button) First color is background color, second color is text color. If not filled in or set to null, default colors will stay
                Color.cyan, Color.white,
                //(Back button in nested page) First color is background color, second color is text color. If not filled in or set to null, default colors will stay
                Color.cyan, Color.yellow
            );

            //Moves the back button one to the right for more room
            Q_Functions.getBackButton().setLocation(2, 0);


            //Q_Functions is initially set as a QMNestedButton, so we want to instantiate 
            //a new one with all of the properties that we want
            Q_F_Gain = new QMToggleButton(
                //BtnMenu -> this has 3 possible types, "ShortcutMenu", "UserInteractMenu" or a QMNestedButton (button that opens up to other buttons)
                //ShortcutMenu -> main quick menu with worlds/avatars
                //UserInteractMenu -> quick menu when selecting a player
                Q_Functions,
                //The x and y coordinates of the button. 0,0 is considered one to the left of worlds button.
                //x -> positive moves you to the right
                //y -> positive moves you down
                1, 0,
                //The text for the 'on' position of the button
                "Max Gain",
                //The 'on' action of the button
                delegate ()
                {
                    Console.WriteLine("Max Gain");
                },
                //The text for the 'off' position of the button
                "Normal Gain",
                //The 'off' action of the button
                delegate ()
                {
                    Console.WriteLine("Normal Gain");
                },
                //The tooltip of the button (the text on top when hovering over the button)
                "Volume to the MAXIMUM!",
                //First color is background color, second color is text color. If not filled in or set to null, default colors will stay
                Color.cyan, Color.white
           );
        }
    }

    public class QMButtonBase
    {
        protected GameObject button;
        protected string btnQMLoc;
        protected string btnType;
        protected string btnTag;
        protected int[] initShift = { 0, 0 };

        public GameObject getGameObject()
        {
            return button;
        }

        public void setActive(bool isActive)
        {
            button.gameObject.SetActive(isActive);
        }

        public void setLocation(int buttonXLoc, int buttonYLoc)
        {
            button.GetComponent<RectTransform>().anchoredPosition += Vector2.right * (420 * (buttonXLoc + initShift[0]));
            button.GetComponent<RectTransform>().anchoredPosition += Vector2.down * (420 * (buttonYLoc + initShift[1]));

            btnTag = "(" + buttonXLoc + "," + buttonYLoc + ")";
            button.name = btnQMLoc + "/" + btnType + btnTag;
            button.GetComponent<Button>().name = btnType + btnTag;
        }

        public void setToolTip(string buttonToolTip)
        {
            button.GetComponent<UiTooltip>().text = buttonToolTip;
            button.GetComponent<UiTooltip>().alternateText = buttonToolTip;
        }
    }

    public class QMSingleButton : QMButtonBase
    {

        public QMSingleButton(QMNestedButton btnMenu, int btnXLocation, int btnYLocation, String btnText, UnityAction btnAction, String btnToolTip, Nullable<Color> btnBackgroundColor = null, Nullable<Color> btnTextColor = null)
        {
            btnQMLoc = btnMenu.getMenuName();
            btnType = "SingleButton";
            initButton(btnXLocation, btnYLocation, btnText, btnAction, btnToolTip, btnBackgroundColor, btnTextColor);
        }

        public QMSingleButton(string btnMenu, int btnXLocation, int btnYLocation, String btnText, UnityAction btnAction, String btnToolTip, Nullable<Color> btnBackgroundColor = null, Nullable<Color> btnTextColor = null)
        {
            btnQMLoc = btnMenu;
            btnType = "SingleButton";
            initButton(btnXLocation, btnYLocation, btnText, btnAction, btnToolTip, btnBackgroundColor, btnTextColor);
        }


        private void initButton(int btnXLocation, int btnYLocation, String btnText, UnityAction btnAction, String btnToolTip, Nullable<Color> btnBackgroundColor = null, Nullable<Color> btnTextColor = null)
        {
            Transform btnTemplate = null;
            btnTemplate = QuickMenuStuff.GetQuickMenuInstance().transform.Find("ShortcutMenu/WorldsButton");

            button = UnityEngine.Object.Instantiate<GameObject>(btnTemplate.gameObject, QuickMenuStuff.GetQuickMenuInstance().transform.Find(btnQMLoc), true);

            initShift[0] = -1;
            initShift[1] = 0;
            setLocation(btnXLocation, btnYLocation);
            setButtonText(btnText);
            setToolTip(btnToolTip);
            setAction(btnAction);

            if (btnBackgroundColor != null)
                setBackgroundColor((Color)btnBackgroundColor);
            if (btnTextColor != null)
                setTextColor((Color)btnTextColor);

            setActive(true);
        }

        public void setButtonText(string buttonText)
        {
            button.GetComponentInChildren<Text>().text = buttonText;
        }

        public void setAction(UnityAction buttonAction)
        {
            button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            button.GetComponent<Button>().onClick.AddListener(buttonAction);
        }

        public void setBackgroundColor(Color buttonBackgroundColor)
        {
            button.GetComponentInChildren<UnityEngine.UI.Image>().color = buttonBackgroundColor;
        }

        public void setTextColor(Color buttonTextColor)
        {
            button.GetComponentInChildren<Text>().color = buttonTextColor;
        }
    }

    public class QMToggleButton : QMButtonBase
    {

        public GameObject btnOn;
        public GameObject btnOff;


        public QMToggleButton(QMNestedButton btnMenu, int btnXLocation, int btnYLocation, String btnTextOn, UnityAction btnActionOn, String btnTextOff, UnityAction btnActionOff, String btnToolTip, Nullable<Color> btnBackgroundColor = null, Nullable<Color> btnTextColor = null)
        {
            btnQMLoc = btnMenu.getMenuName();
            btnType = "ToggleButton";
            initButton(btnXLocation, btnYLocation, btnTextOn, btnActionOn, btnTextOff, btnActionOff, btnToolTip, btnBackgroundColor, btnTextColor);
        }

        public QMToggleButton(string btnMenu, int btnXLocation, int btnYLocation, String btnTextOn, UnityAction btnActionOn, String btnTextOff, UnityAction btnActionOff, String btnToolTip, Nullable<Color> btnBackgroundColor = null, Nullable<Color> btnTextColor = null)
        {
            btnQMLoc = btnMenu;
            btnType = "ToggleButton";
            initButton(btnXLocation, btnYLocation, btnTextOn, btnActionOn, btnTextOff, btnActionOff, btnToolTip, btnBackgroundColor, btnTextColor);
        }

        private void initButton(int btnXLocation, int btnYLocation, String btnTextOn, UnityAction btnActionOn, String btnTextOff, UnityAction btnActionOff, String btnToolTip, Nullable<Color> btnBackgroundColor = null, Nullable<Color> btnTextColor = null)
        {
            Transform btnTemplate = null;
            btnTemplate = QuickMenuStuff.GetQuickMenuInstance().transform.Find("UserInteractMenu/BlockButton");

            button = UnityEngine.Object.Instantiate<GameObject>(btnTemplate.gameObject, QuickMenuStuff.GetQuickMenuInstance().transform.Find(btnQMLoc), true);

            btnOn = button.transform.Find("Toggle_States_Visible/ON").gameObject;
            btnOff = button.transform.Find("Toggle_States_Visible/OFF").gameObject;

            initShift[0] = -4;
            initShift[1] = 0;
            setLocation(btnXLocation, btnYLocation);

            setOnText(btnTextOn);
            setOffText(btnTextOff);
            Text[] btnTextsOn = btnOn.GetComponentsInChildren<Text>();
            btnTextsOn[0].name = "Text_ON";
            btnTextsOn[1].name = "Text_OFF";
            Text[] btnTextsOff = btnOff.GetComponentsInChildren<Text>();
            btnTextsOff[0].name = "Text_ON";
            btnTextsOff[1].name = "Text_OFF";

            setToolTip(btnToolTip);
            button.transform.GetComponentInChildren<UiTooltip>().SetToolTipBasedOnToggle();

            setAction(btnActionOn, btnActionOff);
            btnOn.SetActive(false);
            btnOff.SetActive(true);

            if (btnBackgroundColor != null)
                setBackgroundColor((Color)btnBackgroundColor);

            if (btnTextColor != null)
                setTextColor((Color)btnTextColor);

            setActive(true);

        }

        public void setBackgroundColor(Color buttonBackgroundColor)
        {
            UnityEngine.UI.Image[] btnBgColorList = ((btnOn.GetComponentsInChildren<UnityEngine.UI.Image>()).Concat(btnOff.GetComponentsInChildren<UnityEngine.UI.Image>()).ToArray()).Concat(button.GetComponentsInChildren<UnityEngine.UI.Image>()).ToArray();
            foreach (UnityEngine.UI.Image btnBackground in btnBgColorList) btnBackground.color = buttonBackgroundColor;
        }

        public void setTextColor(Color buttonTextColor)
        {
            Text[] btnTxtColorList = (btnOn.GetComponentsInChildren<Text>()).Concat(btnOff.GetComponentsInChildren<Text>()).ToArray();
            foreach (Text btnText in btnTxtColorList) btnText.color = buttonTextColor;
        }

        public void setAction(UnityAction buttonOnAction, UnityAction buttonOffAction)
        {
            button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            button.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                if (btnOn.activeSelf)
                {
                    buttonOffAction.Invoke();
                    btnOn.SetActive(false);
                    btnOff.SetActive(true);
                }
                else
                {
                    buttonOnAction.Invoke();
                    btnOff.SetActive(false);
                    btnOn.SetActive(true);
                }
            });
        }

        public void setOnText(string buttonOnText)
        {
            Text[] btnTextsOn = btnOn.GetComponentsInChildren<Text>();
            btnTextsOn[0].text = buttonOnText;
            Text[] btnTextsOff = btnOff.GetComponentsInChildren<Text>();
            btnTextsOff[0].text = buttonOnText;
        }

        public void setOffText(string buttonOffText)
        {
            Text[] btnTextsOn = btnOn.GetComponentsInChildren<Text>();
            btnTextsOn[1].text = buttonOffText;
            Text[] btnTextsOff = btnOff.GetComponentsInChildren<Text>();
            btnTextsOff[1].text = buttonOffText;
        }
    }

    public class QMNestedButton
    {
        protected QMSingleButton mainButton;
        protected QMSingleButton backButton;
        protected string menuName;
        protected string btnQMLoc;
        protected string btnType;

        public QMNestedButton(QMNestedButton btnMenu, int btnXLocation, int btnYLocation, String btnText, String btnToolTip, Nullable<Color> btnBackgroundColor = null, Nullable<Color> btnTextColor = null, Nullable<Color> backbtnBackgroundColor = null, Nullable<Color> backbtnTextColor = null)
        {
            btnQMLoc = btnMenu.getMenuName();
            btnType = "NestedButton";
            initButton(btnXLocation, btnYLocation, btnText, btnToolTip, btnBackgroundColor, btnTextColor, backbtnBackgroundColor, backbtnTextColor);
        }

        public QMNestedButton(string btnMenu, int btnXLocation, int btnYLocation, String btnText, String btnToolTip, Nullable<Color> btnBackgroundColor = null, Nullable<Color> btnTextColor = null, Nullable<Color> backbtnBackgroundColor = null, Nullable<Color> backbtnTextColor = null)
        {
            btnQMLoc = btnMenu;
            btnType = "NestedButton";
            initButton(btnXLocation, btnYLocation, btnText, btnToolTip, btnBackgroundColor, btnTextColor, backbtnBackgroundColor, backbtnTextColor);
        }

        public void initButton(int btnXLocation, int btnYLocation, String btnText, String btnToolTip, Nullable<Color> btnBackgroundColor = null, Nullable<Color> btnTextColor = null, Nullable<Color> backbtnBackgroundColor = null, Nullable<Color> backbtnTextColor = null)
        {
            Transform menu = UnityEngine.Object.Instantiate<Transform>(QuickMenuStuff.GetQuickMenuInstance().transform.Find("CameraMenu"), QuickMenuStuff.GetQuickMenuInstance().transform);
            menuName = "CustomMenu" + btnQMLoc + "_" + btnXLocation + "_" + btnYLocation;
            menu.name = menuName;

            mainButton = new QMSingleButton(btnQMLoc, btnXLocation, btnYLocation, btnText, delegate () { QuickMenuStuff.ShowQuickmenuPage(menuName); }, btnToolTip, btnBackgroundColor, btnTextColor);

            IEnumerator enumerator = menu.transform.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object obj = enumerator.Current;
                Transform btnEnum = (Transform)obj;
                if (btnEnum != null)
                {
                    UnityEngine.Object.Destroy(btnEnum.gameObject);
                }
            }

            if (backbtnTextColor == null)
            {
                backbtnTextColor = Color.yellow;
            }
            backButton = new QMSingleButton(this, 4, 2, "Back", delegate () { QuickMenuStuff.ShowQuickmenuPage(btnQMLoc); }, "Go Back", backbtnBackgroundColor, backbtnTextColor);
        }

        public string getMenuName()
        {
            return menuName;
        }

        public QMSingleButton getMainButton()
        {
            return mainButton;
        }

        public QMSingleButton getBackButton()
        {
            return backButton;
        }
    }

    public class QuickMenuStuff : MonoBehaviour
    {
        // <3 VRCTools
        private static VRCUiManager vrcuimInstance;
        public static VRCUiManager GetVRCUiMInstance()
        {
            if (vrcuimInstance == null)
            {
                MethodInfo method = typeof(VRCUiManager).GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public);
                if (method == null)
                {
                    return null;
                }
                vrcuimInstance = (VRCUiManager)method.Invoke(null, new object[0]);
            }
            return vrcuimInstance;
        }


        private static QuickMenu quickmenuInstance;
        public static QuickMenu GetQuickMenuInstance()
        {
            if (quickmenuInstance == null)
            {
                MethodInfo method = typeof(QuickMenu).GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public);
                if (method == null)
                {
                    return null;
                }
                quickmenuInstance = (QuickMenu)method.Invoke(null, new object[0]);
            }
            return quickmenuInstance;
        }

        private static FieldInfo currentPageGetter;
        private static FieldInfo quickmenuContextualDisplayGetter;
        public static void ShowQuickmenuPage(string pagename)
        {
            QuickMenu quickMenuInstance = GetQuickMenuInstance();
            Transform transform = (quickMenuInstance != null) ? quickMenuInstance.transform.Find(pagename) : null;
            if (transform == null)
            {
                Console.WriteLine("[QuickMenuUtils] pageTransform is null !");
            }
            if (currentPageGetter == null)
            {
                if (currentPageGetter == null)
                {
                    GameObject gameObject = quickMenuInstance.transform.Find("ShortcutMenu").gameObject;
                    FieldInfo[] array = (from fi in typeof(QuickMenu).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                         where fi.FieldType == typeof(GameObject)
                                         select fi).ToArray<FieldInfo>();
                    //Console.WriteLine("[QuickMenuUtils] GameObject Fields in QuickMenu:");
                    int num = 0;
                    foreach (FieldInfo fieldInfo in array)
                    {
                        if (fieldInfo.GetValue(quickMenuInstance) as GameObject == gameObject && ++num == 2)
                        {
                            //Console.WriteLine("[QuickMenuUtils] currentPage field: " + fieldInfo.Name);
                            currentPageGetter = fieldInfo;
                            break;
                        }
                    }
                }
                if (currentPageGetter == null)
                {
                    GameObject gameObject = quickMenuInstance.transform.Find("UserInteractMenu").gameObject;
                    FieldInfo[] array = (from fi in typeof(QuickMenu).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                         where fi.FieldType == typeof(GameObject)
                                         select fi).ToArray<FieldInfo>();
                    //Console.WriteLine("[QuickMenuUtils] GameObject Fields in QuickMenu:");
                    int num = 0;
                    foreach (FieldInfo fieldInfo in array)
                    {
                        if (fieldInfo.GetValue(quickMenuInstance) as GameObject == gameObject && ++num == 2)
                        {
                            //Console.WriteLine("[QuickMenuUtils] currentPage field: " + fieldInfo.Name);
                            currentPageGetter = fieldInfo;
                            break;
                        }
                    }
                }

                if (currentPageGetter == null)
                {
                    Console.WriteLine("[QuickMenuUtils] Unable to find field currentPage in QuickMenu");
                    return;
                }
            }
            GameObject gameObject2 = (GameObject)currentPageGetter.GetValue(quickMenuInstance);
            if (gameObject2 != null)
            {
                gameObject2.SetActive(false);
            }
            GetQuickMenuInstance().transform.Find("QuickMenu_NewElements/_InfoBar").gameObject.SetActive(false);
            if (quickmenuContextualDisplayGetter != null)
            {
                quickmenuContextualDisplayGetter = typeof(QuickMenu).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault((FieldInfo fi) => fi.FieldType == typeof(QuickMenuContextualDisplay));
            }
            FieldInfo fieldInfo2 = quickmenuContextualDisplayGetter;
            QuickMenuContextualDisplay quickMenuContextualDisplay = ((fieldInfo2 != null) ? fieldInfo2.GetValue(quickMenuInstance) : null) as QuickMenuContextualDisplay;
            if (quickMenuContextualDisplay != null)
            {
                currentPageGetter.SetValue(quickMenuInstance, transform.gameObject);
                MethodBase method = typeof(QuickMenuContextualDisplay).GetMethod("SetDefaultContext", BindingFlags.Instance | BindingFlags.Public);
                object obj = quickMenuContextualDisplay;
                object[] array3 = new object[3];
                array3[0] = 0;
                method.Invoke(obj, array3);
            }
            currentPageGetter.SetValue(quickMenuInstance, transform.gameObject);
            MethodBase method2 = typeof(QuickMenu).GetMethod("SetContext", BindingFlags.Instance | BindingFlags.Public);
            object obj2 = quickMenuInstance;
            object[] array4 = new object[3];
            array4[0] = 1;
            method2.Invoke(obj2, array4);
            transform.gameObject.SetActive(true);
        }

        public VRCUiPage GetPage(string path)
        {
            GameObject gameObject = GameObject.Find(path);
            VRCUiPage vrcuiPage = null;
            if (gameObject != null)
            {
                vrcuiPage = gameObject.GetComponent<VRCUiPage>();
                if (vrcuiPage == null)
                {
                    UnityEngine.Debug.LogError("Screen Not Found - " + path);
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("Screen Not Found - " + path);
            }
            return vrcuiPage;
        }
    }


}