namespace Delight.Component.MovingLight.Effects.Setters
{
    /// <summary>
    /// 이전 효과 모음와 다음 효과 모음을 잇습니다.
    /// </summary>
    public class ContinueSetter : BaseSetter
    {
        /// <summary>
        /// 이전 효과와 다음 효과에서의 진행할 시간을 나타냅니다.
        /// </summary>
        public int ContinueMilliseconds { get; set; }
    }
}
