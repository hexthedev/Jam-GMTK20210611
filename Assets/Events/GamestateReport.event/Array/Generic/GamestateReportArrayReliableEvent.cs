using HexUN.Events;

namespace GMTK2021
{
   [System.Serializable]
   public class GamestateReportArrayReliableEvent : ReliableEvent<GamestateReport[], GamestateReportArrayUnityEvent>
   {
   }
}