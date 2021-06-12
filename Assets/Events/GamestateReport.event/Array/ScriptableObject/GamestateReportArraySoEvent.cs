using UnityEngine;
using HexUN.Events;

namespace GMTK2021
{
   [CreateAssetMenu(fileName = "GamestateReportArraySoEvent", menuName = "GMTK2021/GamestateReportArray")]
   public class GamestateReportArraySoEvent : ScriptableObjectEvent<GamestateReport[]>
   {
   }
}