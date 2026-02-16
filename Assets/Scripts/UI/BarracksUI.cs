using UnityEngine;
using UnityEngine.UI;

public class BarracksUI : MonoBehaviour
{
    [SerializeField] private Button _trainButton;
    [SerializeField] private bool _closeAfterTraining = false; // Optional: set to true if you want UI to close after training

    private Barracks _current;

    private void Awake()
    {
        gameObject.SetActive(false); // hidden by default until a Barracks is selected
    }

    private void Update()
    {
        // Close UI when ESC is pressed
        if (Input.GetKeyDown(KeyCode.Escape) && gameObject.activeSelf)
        {
            Hide();
        }
    }

    public void ShowFor(Barracks b)
    {
        _current = b;
        gameObject.SetActive(true);

        _trainButton.onClick.RemoveAllListeners();
        _trainButton.onClick.AddListener(OnTrainClicked);
    }

    public void Hide()
    {
        if (_trainButton != null)
        {
            _trainButton.onClick.RemoveAllListeners();
        }
        
        _current = null;
        gameObject.SetActive(false);
    }

    private void OnTrainClicked()
    {
        if (_current == null)
        {
            return;
        }
        
        _current.TrainWarrior();

        // Optional: Close UI after training
        if (_closeAfterTraining)
        {
            Hide();
        }
    }
    
    // Optional: Add a close button functionality
    public void OnCloseButtonClicked()
    {
        Hide();
    }
}