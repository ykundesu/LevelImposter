﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LevelImposter.Core
{
    public class LIStar : MonoBehaviour
    {
        public LIStar(IntPtr intPtr) : base(intPtr)
        {
        }

        private float _height = 10;
        private float _length = 10;
        private float _minSpeed = 2;
        private float _maxSpeed = 2;
        private float _currentSpeed = 0;

        public void Init(LIElement elem)
        {
            if (elem.properties.starfieldHeight != null)
                _height = (float)elem.properties.starfieldHeight;
            if (elem.properties.starfieldLength != null)
                _length = (float)elem.properties.starfieldLength;
            if (elem.properties.starfieldMinSpeed != null)
                _minSpeed = (float)elem.properties.starfieldMinSpeed;
            if (elem.properties.starfieldMaxSpeed != null)
                _maxSpeed = (float)elem.properties.starfieldMaxSpeed;
            Respawn(true);
        }

        public void Respawn(bool isInitial)
        {
            _currentSpeed = UnityEngine.Random.Range(_minSpeed, _maxSpeed);
            transform.localPosition = new Vector3(
                isInitial ? UnityEngine.Random.Range(-_length, 0) : 0,
                UnityEngine.Random.Range(-_height / 2, _height / 2),
                0
            );
        }

        public void Update()
        {
            transform.localPosition -= new Vector3(
                _currentSpeed * Time.deltaTime,
                0,
                0
            );
            if (transform.localPosition.x < -_length)
                Respawn(false);
        }
    }
}