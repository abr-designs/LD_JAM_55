using Managers;
using UnityEngine;

namespace Utilities
{
    public class MousePointer : MonoBehaviour
    {
        [SerializeField]
        private Color32 handColor;
        [SerializeField]
        private SpriteRenderer handSpriteRenderer;
        [SerializeField]
        private SpriteRenderer armSpriteRenderer;
        [SerializeField]
        private Sprite openHand, closedHand;

        [SerializeField, Space(10f)]
        private SpriteRenderer underHandSpriteRenderer;
        [SerializeField]
        private Sprite openHandUnder, closedHandUnder;

        private GameManager _gameManager;

        //Unity Functions
        //============================================================================================================//

        private void OnEnable()
        {
            Cursor.visible = false;
            GameManager.OnGameLost += OnGameLost;
        }

        // Start is called before the first frame update
        private void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
            handSpriteRenderer.color = handColor;
            underHandSpriteRenderer.color = handColor;
            armSpriteRenderer.color = handColor;
        
            SetHandState(true);
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SetHandState(false);
            }
        
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                SetHandState(true);
            }

            transform.position = _gameManager.MouseWorldPosition;
        }
    
        private void OnDisable()
        {
            Cursor.visible = true;
            GameManager.OnGameLost -= OnGameLost;
        }
    
        //============================================================================================================//

        private void SetHandState(bool isOpen)
        {
            handSpriteRenderer.sprite = isOpen ? openHand : closedHand;

            underHandSpriteRenderer.sprite = isOpen ? openHandUnder : closedHandUnder;
        }
    
        //Callbacks
        //============================================================================================================//

        private void OnGameLost()
        {
            Cursor.visible = true;
            enabled = false;
        }

        //============================================================================================================//
    }
}
