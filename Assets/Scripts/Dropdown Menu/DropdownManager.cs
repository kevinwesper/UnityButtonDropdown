using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ButtonDropdown
{
    /// <summary>
    /// Orientation of the dropdown.
    /// </summary>
    public enum Orientation
    {
        Vertical, Horizontal
    }

    [ExecuteInEditMode]
    public class DropdownManager : MonoBehaviour
    {
        [Header ("Main Settings")]
        [SerializeField] private Orientation orientation = Orientation.Vertical;
        [SerializeField] private Color backdropColor = Color.white;
        /*[SerializeField]*/ private float backdropOffset = 5f;

        public GameObject backdropObject;           // Can this be encapsulated?
        public GameObject subButtonManager;         // Can this be encapsulated?

        private RectTransform backdropTransform;
        private RectTransform buttonTransform;

        private Vector3 initialScale;   // WIP
        private Vector3 finalScale;     // WIP

        private bool set = false;       // WIP

        #region Editor

        [Header("Testing")]
        [SerializeField] private bool open_close;

        private int childcount = 0;
        private float x;
        private float y;

        /// <summary>
        /// Used to test in the editor
        /// </summary>
        private void Update()
        {
#if UNITY_EDITOR
            // Update dropdown.
            backdropObject.GetComponent<Image>().color = backdropColor;
            ChangeLayout();

            // Change Backdrop Size if amount of buttons changes.
            if (childcount != subButtonManager.transform.childCount)
            {
                UpdateBackdrop();

                childcount = subButtonManager.transform.childCount;
            }

            // Activate dropdown.
            if (open_close)
            {
                open_close = false;

                Dropdown();
            }
#endif
        }

        #endregion

        /// <summary>
        /// Actual setting of the menu.
        /// </summary>
        private void Start()
        {
            backdropTransform = backdropObject.GetComponent<RectTransform>();
            backdropTransform.sizeDelta = new Vector2(backdropOffset * 2, backdropOffset);

            backdropObject.GetComponent<Image>().color = backdropColor;
            
            backdropObject.SetActive(false);

            for (int i = 0; i < subButtonManager.transform.childCount; i++)
            {
                buttonTransform = subButtonManager.transform.GetChild(i).GetComponent<RectTransform>();

                subButtonManager.transform.GetChild(i).gameObject.SetActive(false);

                backdropTransform.sizeDelta = new Vector2(buttonTransform.sizeDelta.x + backdropOffset * 2, backdropTransform.sizeDelta.y + buttonTransform.sizeDelta.y + backdropOffset);
            }

            /*Testing*/
            childcount = subButtonManager.transform.childCount;

            ChangeLayout();
        }

        /// <summary>
        /// Changes the axis of the button layout and arrow direction.
        /// </summary>
        private void ChangeLayout()
        {
            GridLayoutGroup grid = subButtonManager.GetComponent<GridLayoutGroup>();

            switch (orientation)
            {
                case Orientation.Vertical:
                    grid.startAxis = GridLayoutGroup.Axis.Horizontal;
                    break;
                case Orientation.Horizontal:
                    grid.startAxis = GridLayoutGroup.Axis.Vertical;
                    break;
            }

            UpdateBackdrop();
        }

        /// <summary>
        /// Update backdrop shape.
        /// </summary>
        private void UpdateBackdrop()
        {
            float wide = 0;
            float high = 0;

            if (orientation == Orientation.Vertical)
            {
                backdropTransform.sizeDelta = new Vector2(backdropOffset * 2, backdropOffset);

                for (int i = 0; i < subButtonManager.transform.childCount; i++)
                {
                    buttonTransform = subButtonManager.transform.GetChild(i).GetComponent<RectTransform>();

                    wide = (wide < buttonTransform.sizeDelta.x) ? buttonTransform.sizeDelta.x : wide;

                    backdropTransform.sizeDelta = new Vector2(buttonTransform.sizeDelta.x + backdropOffset * 2, backdropTransform.sizeDelta.y + buttonTransform.sizeDelta.y + backdropOffset);
                }

                if (!set)                                           // WIP
                {
                    initialScale = new Vector3(1, 0, 1);
                    finalScale = backdropTransform.localScale;
                    set = true;
                }
            }
            else if (orientation == Orientation.Horizontal)
            {
                backdropTransform.sizeDelta = new Vector2(backdropOffset, backdropOffset);

                for (int i = 0; i < subButtonManager.transform.childCount; i++)
                {
                    buttonTransform = subButtonManager.transform.GetChild(i).GetComponent<RectTransform>();

                    high = (high < buttonTransform.sizeDelta.y) ? buttonTransform.sizeDelta.y : high;

                    backdropTransform.sizeDelta = new Vector2(backdropTransform.sizeDelta.x + buttonTransform.sizeDelta.x + backdropOffset, buttonTransform.sizeDelta.y + backdropOffset * 2);
                }

                if (!set)                                           // WIP
                {
                    initialScale = new Vector3(0, 1, 1);
                    finalScale = backdropTransform.localScale;
                    set = true;
                }
            }
            else
            {
                Debug.LogError("Something is wrong with the orientation. Have you added another orientation method?");
            }
        }

        private IEnumerator OpenClose(bool open)    // WIP
        {
            float progress = 0;

            Debug.LogWarning("Initial: " + initialScale);
            Debug.LogWarning("Final: " + finalScale);

            backdropObject.SetActive(!backdropObject.activeSelf);

            while (progress <= 1)
            {
                backdropTransform.localScale = open ? Vector3.Lerp(initialScale, finalScale, progress) : Vector3.Lerp(finalScale, initialScale, progress);
                progress += Time.deltaTime * 2;
                yield return null;
            }
            backdropTransform.localScale = finalScale;

            Debug.LogWarning("Initial: " + initialScale);
            Debug.LogWarning("Final: " + finalScale);

            yield return new WaitForSeconds(5);
            print(Time.time);
        }

        /// <summary>
        /// Activates the backdrop and buttons.
        /// </summary>
        public void Dropdown()
        {
            StartCoroutine(OpenClose(!backdropObject.activeSelf));  // WIP

            for (int i = 0; i < subButtonManager.transform.childCount; i++)
            {
                subButtonManager.transform.GetChild(i).gameObject.SetActive(!subButtonManager.transform.GetChild(i).gameObject.activeSelf);
            }
        }
    }
}
