using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MachineProgressDisplay : MonoBehaviour
{
    public PlaceableMachine MachineRef;
    public Image FillImage;
    public CanvasGroup CanvasGroup;

    private bool _destroyed;

    private void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
        CanvasGroup.alpha = 1.0F;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        _destroyed = false;
    }

    private void Update()
    {
        if (MachineRef != null) 
        {
            FillImage.fillAmount = MachineRef.MachineProgress;
            transform.position = MachineRef.transform.position + new Vector3(0, 3, 0);
        }

        if (FillImage.fillAmount >= 1.0f && !_destroyed)
        {
            _destroyed = true;
            DelayDestruction().Forget();
        } 
    }

    private async UniTask DelayDestruction() 
    {
        await UniTask.Delay(200);
        BiomePhaseHandler.Instance.ProcessMachineImpact(MachineRef);
        if(gameObject != null) Destroy(gameObject);
    }

}
