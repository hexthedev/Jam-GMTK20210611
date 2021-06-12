using HexUN.Events;
using UnityEngine;
using UnityEngine.Events;

namespace GMTK2021
{
   [AddComponentMenu("GMTK2021/GamestateReport/GamestateReportSoEventListener")]
   public class GamestateReportSoEventListener : ScriptableObjectEventListener<GamestateReport, GamestateReportSoEvent, GamestateReportUnityEvent>
   {
   }
}