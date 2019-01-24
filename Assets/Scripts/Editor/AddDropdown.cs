using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEditor.Events;

namespace ButtonDropdown
{
    public class AddDropdown : MonoBehaviour
    {
        [MenuItem("GameObject/UI/Button Dropdown", false, 10)]
        public static void AddButtonDD(MenuCommand menuCommand)
        {
            // Get or create a Canvas.
            GameObject parent = getsetCanvas();

            // Get or create an EventSystem.
            GameObject es = getsetEventSystem();

            // Create the Dropdown.
            GameObject buttonDropdown = createDropdown(parent);
        }

        /// <summary>
        /// Checks if canvas is present, if not, create one.
        /// </summary>
        /// <returns>Canvas Object</returns>
        static public GameObject getsetCanvas()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            GameObject root;

            if (canvas == null)
            {
                root = new GameObject("Canvas");
                root.layer = LayerMask.NameToLayer("UI");
                canvas = root.AddComponent<Canvas>();
                root.AddComponent<CanvasScaler>();
                root.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
            }
            else
            {
                root = canvas.gameObject;
            }

            return root;
        }

        /// <summary>
        /// Checks if EventSystem is present, if not, create one.
        /// </summary>
        /// <returns>EventSystem Object</returns>
        static public GameObject getsetEventSystem()
        {
            EventSystem es = FindObjectOfType<EventSystem>();
            GameObject root;

            if (es == null)
            {
                root = new GameObject("EventSystem");
                root.layer = LayerMask.NameToLayer("UI");
                es = root.AddComponent<EventSystem>();
                root.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
            }
            else
            {
                root = es.gameObject;
            }

            return root;
        }

        /// <summary>
        /// Create the dropdown menu from scratch.
        /// </summary>
        /// <param name="parent">The canvas parent</param>
        /// <returns>Menu Object</returns>
        static public GameObject createDropdown(GameObject parent)
        {
            // Create head first, some variables may be filled in later.
            GameObject head = new GameObject("Button Dropdown");
            GameObjectUtility.SetParentAndAlign(head, parent);
            Selection.activeObject = head;
            head.AddComponent<RectTransform>();
            DropdownManager manager = head.AddComponent<DropdownManager>();
            Undo.RegisterCreatedObjectUndo(head, "Create " + head.name);

            // Create backdrop
            GameObject backdrop = new GameObject("Backdrop");                                                 // location and size broken???
            GameObjectUtility.SetParentAndAlign(backdrop, head);
            RectTransform itemTransform = backdrop.AddComponent<RectTransform>();
            itemTransform.anchorMin = new Vector2(0,1);
            itemTransform.anchorMax = new Vector2(0,1);
            itemTransform.pivot = new Vector2(0,1);
            itemTransform.localPosition = new Vector2(-55f, 50f);
            backdrop.AddComponent<CanvasRenderer>();
            Image backdropImage = backdrop.AddComponent<Image>();
            backdropImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            backdropImage.type = Image.Type.Sliced;
            backdrop.SetActive(false);
            manager.backdropObject = backdrop;
            Undo.RegisterCompleteObjectUndo(backdrop, "Create " + backdrop.name);

            // Create open button
            GameObject button = createButton(head, true, true);
            button.name = "Open Button";
            Button function = button.GetComponent<Button>();
            UnityEventTools.AddPersistentListener(function.onClick, manager.Dropdown);
            GameObject buttonText = createText(button, "Open");

            // Create submanager
            GameObject subManager = new GameObject("Manager");
            GameObjectUtility.SetParentAndAlign(subManager, head);
            itemTransform = subManager.AddComponent<RectTransform>();
            itemTransform.anchorMin = new Vector2(0.5f, 1);
            itemTransform.anchorMax = new Vector2(0.5f, 1);
            itemTransform.pivot = new Vector2(0.5f, 1);
            itemTransform.localPosition = new Vector2(0f, 45f);
            GridLayoutGroup layout = subManager.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(100,50);
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.spacing = new Vector2(5, 5);
            manager.subButtonManager = subManager;
            Undo.RegisterCompleteObjectUndo(subManager, "Create " + subManager.name);

            // Fill submanager
            button = createButton(subManager, true, false);
            button.name = "Close Button";
            function = button.GetComponent<Button>();
            UnityEventTools.AddPersistentListener(function.onClick, manager.Dropdown);
            buttonText = createText(button, "Close");
            button.SetActive(false);
            for (int i = 0; i < 2; i++)
            {
                button = createButton(subManager, false, false);
                button.name = "Option " + (i+1);
                buttonText = createText(button, "Option " + (i+1));
                button.SetActive(false);
            }

            manager.enabled = true;
            return head;
        }
        
        /// <summary>
        /// Creates a button.
        /// </summary>
        /// <param name="parent">Parent of the button</param>
        /// <param name="image">Arrow yes or no</param>
        /// <param name="open">if yes, Open or Close</param>
        /// <returns>Button</returns>
        static public GameObject createButton(GameObject parent, bool image, bool open)
        {
            GameObject button = new GameObject("Button");
            GameObjectUtility.SetParentAndAlign(button, parent);
            RectTransform bdTransform = button.AddComponent<RectTransform>();
            bdTransform.anchorMin = new Vector2(0.5f, 1);
            bdTransform.anchorMax = new Vector2(0.5f, 1);
            bdTransform.pivot = new Vector2(0.5f, 1);
            bdTransform.localPosition = new Vector2(0, 45f);
            bdTransform.sizeDelta = new Vector2(100f, 50f);
            button.AddComponent<CanvasRenderer>();
            Image bdImage = button.AddComponent<Image>();
            bdImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            bdImage.type = Image.Type.Sliced;
            button.AddComponent<Button>();
            Undo.RegisterCompleteObjectUndo(button, "Create " + button.name);

            if (image)
            {
                GameObject child = new GameObject("Arrow");
                GameObjectUtility.SetParentAndAlign(child, button);
                RectTransform childTransform = child.AddComponent<RectTransform>();
                childTransform.anchorMin = new Vector2(0.5f, 0.5f);
                childTransform.anchorMax = new Vector2(0.5f, 0.5f);
                childTransform.pivot = new Vector2(0.5f, 0.5f);
                childTransform.localEulerAngles = open ? new Vector3(0, 0, 90) : new Vector3(0, 0, 270);
                childTransform.localPosition = new Vector2(30, -25);
                childTransform.sizeDelta = new Vector2(20f, 20f);
                child.AddComponent<CanvasRenderer>();
                Image childImage = child.AddComponent<Image>();
                childImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd");
                childImage.type = Image.Type.Sliced;
                Undo.RegisterCompleteObjectUndo(child, "Create " + child.name);
            }

            return button;
        }

        /// <summary>
        /// Creates text for on the button.
        /// </summary>
        /// <param name="button">parent button</param>
        /// <param name="bText">What text should be on the button</param>
        /// <returns>Text object for on the button</returns>
        static public GameObject createText(GameObject button, string bText = "")
        {
            GameObject textObject = new GameObject("Text");
            GameObjectUtility.SetParentAndAlign(textObject, button);
            RectTransform textTransform = textObject.AddComponent<RectTransform>();
            textTransform.anchorMin = new Vector2(0, 0);
            textTransform.anchorMax = new Vector2(1f, 1f);
            textTransform.pivot = new Vector2(0.5f, 0.5f);
            textObject.AddComponent<CanvasRenderer>();
            Text objectText = textObject.AddComponent<Text>();
            objectText.text = bText;
            textTransform.sizeDelta = new Vector2(0,0);
            objectText.color = Color.black;
            objectText.alignment = TextAnchor.MiddleCenter;
            Undo.RegisterCompleteObjectUndo(textObject, "Create " + textObject.name);

            return textObject;
        }
    }
}