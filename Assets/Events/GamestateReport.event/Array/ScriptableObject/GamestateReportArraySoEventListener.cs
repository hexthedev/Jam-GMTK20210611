using HexUN.Events;
using UnityEngine;
using UnityEngine.Events;

namespace GMTK2021
{
   [AddComponentMenu("GMTK2021/GamestateReportArray/GamestateReportArraySoEventListener")]
   public class GamestateReportArraySoEventListener : ScriptableObjectEventListener<GamestateReport[], GamestateReportArraySoEvent, GamestateReportArrayUnityEvent>
   {
   }
}