using System;

namespace DefaultNamespace {

    class PIDAngleController {

        public float proportionalGain;
        public float integralGain;
        public float derivitiveGain;

        private float _errorLast = 0f;
        private float _valueLast = 0f;

        public float UpdateAngle(float dt, float currentAngle, float targetAngle) {
            float error = targetAngle - currentAngle;

            // Calculate P term
            float P = proportionalGain * error;
            
            // Calculate D term
            float errorRateOfChange = AngleDifference(error, _errorLast) / dt;
            _errorLast = error;

            // float valueRateOfChange = AngleDifference(currentAngle, _valueLast) / dt;
            // _valueLast = currentAngle;
            
            float D = derivitiveGain * errorRateOfChange;

            return P + D;
        }

        float AngleDifference(float a, float b) {
            return (a - b + 540) % 360 - 180;
        }

    }

}