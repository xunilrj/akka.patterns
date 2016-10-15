using System;
using System.Collections.Generic;

namespace SomeBank.AkkaUtils.Data
{
    public class DomainDataResult<T> where T : struct
    {
        public static DomainDataResult<T> AsSuccess(T data)
        {
            return new DomainDataResult<T>()
                {
                    Data = data,
                    HasError = false
                };
        }

        public static DomainDataResult<T> AsFailure(params Exception[] errors)
        {
            var result = new DomainDataResult<T>()
            {
                Data = null,
                HasError = true
            };

            result.Errors.AddRange(errors);

            return result;
        }

        public Nullable<T> Data { get; private set; }

        public bool HasError { get; set; }
        public List<Exception> Errors { get; set; }

        DomainDataResult()
        {
            Errors = new List<Exception>();
        }

        public static implicit operator T (DomainDataResult<T> data)
        {
            if(data.HasError == false)
            {
                return data.Data.Value;
            }
            else
            {
                //TODO think in a better exception
                throw new Exception();
            }
        }
    }
}
