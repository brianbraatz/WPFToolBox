//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: Aggregate functions for PivotTable model.
//  Creator: t-tomkm
//  Date Created: 07/20/2006
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Base, abstract class for all agregate functions.
    /// </summary>
    public abstract class AggregateFunction
    {
        private string _name;
        private static AverageAggregateFunction _averageFunction;
        private static CountAggregateFunction _countFunction;
        private static SumAggregateFunction _sumFunction;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the function.</param>
        protected AggregateFunction(string name)
        {
            Assert.ArgumentNotNull(name, "name", "Name cannot be null. Use an empty string instead.");
            _name = name;
        }

        /// <summary>
        /// Name of the function.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Computes value of the function for given arguments.
        /// </summary>
        /// <param name="arguments">Arguments.</param>
        /// <returns>Value of the function for given arguments.</returns>
        public abstract object ComputeValue(IEnumerable arguments);

        /// <summary>
        /// Gets Average aggregate function.
        /// </summary>
        public static AggregateFunction Average
        {
            get
            {
                if (_averageFunction == null)
                {
                    _averageFunction = new AverageAggregateFunction();
                }
                return _averageFunction;
            }
        }

        /// <summary>
        /// Gets Count aggregate function.
        /// </summary>
        public static AggregateFunction Count
        {
            get
            {
                if (_countFunction == null)
                {
                    _countFunction = new CountAggregateFunction();
                }
                return _countFunction;
            }
        }

        /// <summary>
        /// Gets Sum aggregate function.
        /// </summary>
        public static AggregateFunction Sum
        {
            get
            {
                if (_sumFunction == null)
                {
                    _sumFunction = new SumAggregateFunction();
                }
                return _sumFunction;
            }
        }
    }

    /// <summary>
    /// Base class for all aggregate functions that accepts only numbers as arguments.
    /// </summary>
    public abstract class NumericAggregateFunction : AggregateFunction
    {
        private readonly Type[] TypesConvertibleToInt32 = new Type[] { typeof(Byte), typeof(SByte), typeof(Int32), typeof(Int16), typeof(UInt32), typeof(UInt16) };

        private readonly Type[] TypesConvertibleToSingle = new Type[] { typeof(Single) };

        private readonly Type[] TypesConvertibleToDouble = new Type[] { typeof(Double), typeof(Int64), typeof(UInt64), typeof(Decimal) };

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the function.</param>
        protected NumericAggregateFunction(string name)
            : base(name)
        { }

        /// <summary>
        /// Computes value of the function for given arguments. All arguments must be numbers.
        /// </summary>
        /// <param name="arguments">Arguments.</param>
        /// <returns>Value of the function for given arguments.</returns>
        public override object ComputeValue(IEnumerable arguments)
        {
            Assert.ArgumentNotNull(arguments, "arguments", "Collection of arguments cannot be null.");
            ResultType resultType = GetResultType(arguments);
            switch (resultType)
            {
                case ResultType.Int32:
                    return ComputeValueForInt32Arguments(ConvertArgumentsToInt32(arguments));

                case ResultType.Single:
                    return ComputeValueForSingleArguments(ConvertArgumentsToSingle(arguments));

                case ResultType.Double:
                    return ComputeValueForDoubleArguments(ConvertArgumentsToDouble(arguments));

                default:
                    throw new ArgumentException("Numeric aggregate functions cannot be use for not-numeric arguments.");
            }
        }

        /// <summary>
        /// Returns value of the function for <see cref="System.Int32"/> arguments.
        /// </summary>
        /// <param name="arguments">Arguments for the function.</param>
        /// <returns>Value of the function for <see cref="System.Int32"/> arguments.</returns>
        protected abstract object ComputeValueForInt32Arguments(IEnumerable<int?> arguments);

        /// <summary>
        /// Returns value of the function for <see cref="System.Single"/> arguments.
        /// </summary>
        /// <param name="arguments">Arguments for the function.</param>
        /// <returns>Value of the function for <see cref="System.Single"/> arguments.</returns>
        protected abstract object ComputeValueForSingleArguments(IEnumerable<float?> arguments);

        /// <summary>
        /// Returns value of the function for <see cref="System.Double"/> arguments.
        /// </summary>
        /// <param name="arguments">Arguments for the function.</param>
        /// <returns>Value of the function for <see cref="System.Double"/> arguments.</returns>
        protected abstract object ComputeValueForDoubleArguments(IEnumerable<double?> arguments);

        private ResultType GetResultType(IEnumerable arguments)
        {
            ResultType result = ResultType.Int32;

            foreach (object argument in arguments)
            {
                if (argument != null)
                {
                    if (IsConvertibleToInt32(argument))
                    {
                        result = ChangeResultType(result, ResultType.Int32);
                    }
                    else if (IsConvertibleToSingle(argument))
                    {
                        result = ChangeResultType(result, ResultType.Single);
                    }
                    else if (IsConvertibleToDouble(argument))
                    {
                        result = ChangeResultType(result, ResultType.Double);
                    }
                    else
                    {
                        result = ResultType.NotSupported;
                        break;
                    }
                }
            }

            return result;
        }

        private bool IsConvertibleToInt32(object argument)
        {
            return IsConvertible(argument, TypesConvertibleToInt32);
        }

        private bool IsConvertibleToSingle(object argument)
        {
            return IsConvertible(argument, TypesConvertibleToSingle);
        }

        private bool IsConvertibleToDouble(object argument)
        {
            return IsConvertible(argument, TypesConvertibleToDouble);
        }

        private bool IsConvertible(object argument, Type[] supportedTypes)
        {
            foreach (Type type in supportedTypes)
            {
                if (argument.GetType() == type)
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<Int32?> ConvertArgumentsToInt32(IEnumerable arguments)
        {
            foreach (object argument in arguments)
            {
                yield return argument == null ? null : (int?)Convert.ToInt32(argument);
            }
        }

        private IEnumerable<float?> ConvertArgumentsToSingle(IEnumerable arguments)
        {
            foreach (object argument in arguments)
            {
                yield return argument == null ? null : (float?)Convert.ToSingle(argument);
            }
        }

        private IEnumerable<double?> ConvertArgumentsToDouble(IEnumerable arguments)
        {
            foreach (object argument in arguments)
            {
                yield return argument == null ? null : (double?)Convert.ToDouble(argument);
            }
        }

        private ResultType ChangeResultType(ResultType previousResult, ResultType newArgumentType)
        {
            return previousResult > newArgumentType ? previousResult : newArgumentType;
        }

        private enum ResultType
        {
            NotSupported = 0,
            Int32 = 1,
            Single = 2,
            Double = 3
        }
    }

    /// <summary>
    /// Average aggregate function.
    /// </summary>
    internal class AverageAggregateFunction : NumericAggregateFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AverageAggregateFunction()
            : base("Average")
        { }

        /// <summary>
        /// Returns value of the function for <see cref="System.Int32"/> arguments.
        /// </summary>
        /// <param name="arguments">Arguments for the function.</param>
        /// <returns>Value of the function for <see cref="System.Int32"/> arguments.</returns>
        protected override object ComputeValueForInt32Arguments(IEnumerable<int?> arguments)
        {
            double? result = null;
            int count = 0;
            foreach (int? argument in arguments)
            {
                if (argument.HasValue)
                {
                    result = result.HasValue ? result + argument : argument;
                    count++;
                }
            }
            if (count > 0)
            {
                result /= count;
            }
            return result;
        }

        /// <summary>
        /// Returns value of the function for <see cref="System.Single"/> arguments.
        /// </summary>
        /// <param name="arguments">Arguments for the function.</param>
        /// <returns>Value of the function for <see cref="System.Single"/> arguments.</returns>
        protected override object ComputeValueForSingleArguments(IEnumerable<float?> arguments)
        {
            float? result = null;
            int count = 0;
            foreach (float? argument in arguments)
            {
                if (argument.HasValue)
                {
                    result = result.HasValue ? result + argument : argument;
                    count++;
                }
            }
            if (count > 0)
            {
                result /= count;
            }
            return result;
        }

        /// <summary>
        /// Returns value of the function for <see cref="System.Double"/> arguments.
        /// </summary>
        /// <param name="arguments">Arguments for the function.</param>
        /// <returns>Value of the function for <see cref="System.Double"/> arguments.</returns>
        protected override object ComputeValueForDoubleArguments(IEnumerable<double?> arguments)
        {
            double? result = null;
            int count = 0;
            foreach (double? argument in arguments)
            {
                if (argument.HasValue)
                {
                    result = result.HasValue ? result + argument : argument;
                    count++;
                }
            }
            if (count > 0)
            {
                result /= count;
            }
            return result;
        }
    }

    /// <summary>
    /// Sum aggregate function.
    /// </summary>
    internal class SumAggregateFunction : NumericAggregateFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SumAggregateFunction()
            : base("Sum")
        { }

        /// <summary>
        /// Returns value of the function for <see cref="System.Int32"/> arguments.
        /// </summary>
        /// <param name="arguments">Arguments for the function.</param>
        /// <returns>Value of the function for <see cref="System.Int32"/> arguments.</returns>
        protected override object ComputeValueForInt32Arguments(IEnumerable<int?> arguments)
        {
            int? result = null;
            foreach (int? argument in arguments)
            {
                if (argument.HasValue)
                {
                    result = result.HasValue ? result + argument : argument;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns value of the function for <see cref="System.Single"/> arguments.
        /// </summary>
        /// <param name="arguments">Arguments for the function.</param>
        /// <returns>Value of the function for <see cref="System.Single"/> arguments.</returns>
        protected override object ComputeValueForSingleArguments(IEnumerable<float?> arguments)
        {
            float? result = null;
            foreach (float? argument in arguments)
            {
                if (argument.HasValue)
                {
                    result = result.HasValue ? result + argument : argument;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns value of the function for <see cref="System.Double"/> arguments.
        /// </summary>
        /// <param name="arguments">Arguments for the function.</param>
        /// <returns>Value of the function for <see cref="System.Double"/> arguments.</returns>
        protected override object ComputeValueForDoubleArguments(IEnumerable<double?> arguments)
        {
            double? result = null;
            foreach (double? argument in arguments)
            {
                if (argument.HasValue)
                {
                    result = result.HasValue ? result + argument : argument;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Count aggregate function.
    /// </summary>
    internal class CountAggregateFunction : AggregateFunction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public CountAggregateFunction()
            : base("Count")
        { }

        /// <summary>
        /// Computes value of the function for given arguments.
        /// </summary>
        /// <param name="arguments">Arguments.</param>
        /// <returns>Value of the function for given arguments.</returns>
        public override object ComputeValue(IEnumerable arguments)
        {
            Assert.ArgumentNotNull(arguments, "arguments", "Collection of arguments cannot be null.");
            int result = 0;
            foreach (object argument in arguments)
            {
                if (argument != null)
                {
                    result++;
                }
            }
            return result;
        }
    }
}
