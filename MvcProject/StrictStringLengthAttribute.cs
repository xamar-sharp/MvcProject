using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System;
namespace MvcProject
{
    public class StrictStringLengthAttribute:ValidationAttribute
    {
        private int _requiredLength;
        public StrictStringLengthAttribute(int length)
        {
            _requiredLength = length;
        }
        public override bool IsValid(object value)
        {
            return (value as string)?.Length == _requiredLength;
        }
    }
}
