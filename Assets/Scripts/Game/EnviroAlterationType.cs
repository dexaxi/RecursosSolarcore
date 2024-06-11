using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnviroAlterationType
{
    // A�ADIR SIEMPRE POR ABAJO PARA NO JODER LAS RELACIONES QUE YA HAY
    // Jungle
    Deforestation = 0,
    Farming_Caused_Soil_Degradation,
    // Ocean
    Over_Fishing,
    Water_Temp_Rise,
    // Forest
    Forest_Fires,
    Predator_Lack,
    // Mediterranean
    Desertification,
    Farming_Contamination,

    //UNUSED
    Eutrophication,
    Water_Contamination,
    Morphologic_Damage,
    Permafrost_Unfreeze,
    Precipitation_Change,
    Farming_Soil_Conversion,
    Habitat_Fragmentation_Urban_Development,
}
