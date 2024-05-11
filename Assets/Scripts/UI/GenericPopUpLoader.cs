using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPopUpLoader : MonoBehaviour
{
    public static GenericPopUp LoadGenericPopUp() 
    {
        var GenericPopUpSO = Resources.Load("ScriptableObjects/PopUps/GenericPopUp", typeof(GenericPopUpSO));
        GenericPopUpSO popUpSO = (GenericPopUpSO) GenericPopUpSO;
        Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
        GameObject popUpGO = Instantiate(popUpSO.Instance, center, Quaternion.identity ,GameObject.FindGameObjectsWithTag("PopUpCanvas")[0]?.transform);
        GenericPopUp returnVal = popUpGO.GetComponent<GenericPopUp>();
        return returnVal;
    }

}
