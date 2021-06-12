using HexUN.Events;

namespace GMTK2021
{
   [System.Serializable]
   public class GamestateReportReliableEvent : ReliableEvent<GamestateReport, GamestateReportUnityEvent>
   {
   }
}