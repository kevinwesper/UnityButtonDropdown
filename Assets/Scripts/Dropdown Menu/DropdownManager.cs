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

        /*[SerializeField]*/ private float backdropOffset = 5f;         // WIP > Can I actually do something with this variable?

        public GameObject backdropObject;           // Can this be encapsulated?
        public GameObject subButtonManager;         // Can this be encapsulated?

        private RectTransform backdropTransform;
        private RectTransform buttonTransform;

        private bool open = false;
        private bool opening = false;

        private Vector3 closedScale;
        private Vector3 openScale = new Vector3(1,1,1);

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
                TestDown();
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
                    closedScale = new Vector3(1, 0, 1);
                    break;
                case Orientation.Horizontal:
                    grid.startAxis = GridLayoutGroup.Axis.Vertical;
                    closedScale = new Vector3(0, 1, 1);
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
            }
            else
            {
                Debug.LogError("Something is wrong with the orientation. Have you added another orientation method?");
            }
        }

        /// <summary>
        /// Activates the dropping down animation.
        /// </summary>
        public void Dropdown()
        {
            if (!opening)
            {
                StartCoroutine(DroppingDown());
            }
        }

        /// <summary>
        /// Dropdown for testing purposes in the editor.
        /// </summary>
        private void TestDown()
        {
            backdropObject.SetActive(!backdropObject.activeSelf);
            for (int i = 0; i < subButtonManager.transform.childCount; i++)
            {
                subButtonManager.transform.GetChild(i).gameObject.SetActive(!subButtonManager.transform.GetChild(i).gameObject.activeSelf);
            }
        }

        /// <summary>
        /// Opens or closes the dropdown menu.
        /// </summary>
        /// <returns></returns>
        public IEnumerator DroppingDown()
        {
            float progress = 0;

            Vector3 startLocation;
            Vector3 endLocation;

            if (open)
            {
                for (int i = 0; i < subButtonManager.transform.childCount; i++)
                {
                    subButtonManager.transform.GetChild(i).gameObject.SetActive(!subButtonManager.transform.GetChild(i).gameObject.activeSelf);
                }

                startLocation = openScale;
                endLocation = closedScale;

                open = false;
            }
            else
            {
                startLocation = closedScale;
                endLocation = openScale;

                open = true;
            }

            backdropObject.SetActive(true);

            while (progress <= 1)
            {
                opening = true;
                backdropTransform.localScale = Vector3.Lerp(startLocation, endLocation, progress);
                progress += Time.deltaTime;
                yield return null;
            }
            opening = false;

            if (open)
            {
                for (int i = 0; i < subButtonManager.transform.childCount; i++)
                {
                    subButtonManager.transform.GetChild(i).gameObject.SetActive(!subButtonManager.transform.GetChild(i).gameObject.activeSelf);
                }
            }

            backdropTransform.localScale = endLocation;

            yield return null;
        }
    }
}
