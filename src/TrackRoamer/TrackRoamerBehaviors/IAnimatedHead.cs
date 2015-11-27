using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    public interface IAnimatedHead
    {
        void ClearHeadAnimations();

        void StartHeadAnimationsDefault();

        void StartHeadAnimationCombo(HeadComboAnimations anim, bool repeat = false, double scale = 0.2d);

        void AddHeadAnimationCombo(HeadComboAnimations anim, bool repeat = false, double scale = 0.2d);
    }
}
