using System.Collections.Generic;
using GameScene.Component;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameScene
{
    public class GameController : MonoBehaviour
    {
        [Header("Reference model")] 
        [SerializeField] private GameView view;
        [SerializeField] private GameModel model;

        [Header("Testing only")] 
        [SerializeField] private List<SelectType> generateList;

        // SYSTEM
        private List<Arrow> storeSelector = new();
        private List<Arrow> storeSelected = new();

        private void Awake()
        {
            // Load services
            if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) != null)
            {
                var services = GameObject.FindGameObjectWithTag(Constants.ServicesTag).GetComponent<GameServices>();
            }
            else
            {
                //  SceneManager.LoadScene(Constants.EntryScene);
            }
        }

        private void Start()
        {
            InitScene();
        }

        private void InitScene()
        {
            // Generate objects selector
            foreach (var o in generateList)
            {
                var obj = Instantiate(model.GetSelector(o));
                view.SetParentSelector(obj.transform);
                storeSelector.Add(obj.GetComponent<Arrow>());
            }

            // Assign callback for selector
            foreach (var arrow in storeSelector)
            {
                arrow.Init(OnClickedSelector);
            }
        }


        private void OnClickedSelector(SelectType selectType)
        {
            Debug.Log("Clicked" + selectType);
        }
    }
}