//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: AdomdObjectsRepository class.
//  Creator: t-tomkm
//  Date Created: 7/25/2006
//---------------------------------------------------------------------
using Adomd = Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.WPF.CustomControls.DataModel
{
    /// <summary>
    /// Class that maps source Adomd.NET objects to objects from the PivotTable Model
    /// for Adomd.NET.
    /// </summary>
    internal class AdomdObjectsRepository
    {
        #region Fields
        private Dictionary<Adomd.Level, AdomdLevel> _levelsDictionary;
        private Dictionary<Adomd.Member, AdomdMember> _membersDictionary;
        private Dictionary<string, AdomdMeasuresMember> _measuresNamesDictionary;
        private Dictionary<Tuple, int> _tupleOrdinalPositions;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public AdomdObjectsRepository()
        {
            _levelsDictionary = new Dictionary<Adomd.Level, AdomdLevel>();
            _membersDictionary = new Dictionary<Adomd.Member, AdomdMember>();
            _measuresNamesDictionary = new Dictionary<string, AdomdMeasuresMember>();
            _tupleOrdinalPositions = new Dictionary<Tuple, int>();
        }

        #region Registering source and model elements
        /// <summary>
        /// Associates Adomd.NET level with model level.
        /// </summary>
        /// <param name="sourceLevel">Adomd.NET level.</param>
        /// <param name="modelLevel">Model level.</param>
        public void Register(Adomd.Level sourceLevel, AdomdLevel modelLevel)
        {
            Assert.ArgumentNotNull(sourceLevel, "sourceLevel", "Source level cannot be null.");
            Assert.ArgumentNotNull(modelLevel, "modelLevel", "Model level cannot be null.");
            Assert.ArgumentValid(!_levelsDictionary.ContainsKey(sourceLevel), "sourceLevel",
                "Given source level is already registered.");
            _levelsDictionary[sourceLevel] = modelLevel;
        }

        /// <summary>
        /// Associates Adomd.NET member with model member.
        /// </summary>
        /// <param name="sourceMember">Adomd.NET member.</param>
        /// <param name="modelMember">Model member.</param>
        public void Register(Adomd.Member sourceMember, AdomdMember modelMember)
        {
            Assert.ArgumentNotNull(sourceMember, "sourceMember", "Source member cannot be null.");
            Assert.ArgumentNotNull(modelMember, "modelMember", "Model member cannot be null.");
            Assert.ArgumentValid(!_membersDictionary.ContainsKey(sourceMember), "sourceMember",
                "Given source member is already registered.");
            _membersDictionary[sourceMember] = modelMember;
        }

        /// <summary>
        /// Associates unique measure name with AdomdMeasuresMember object.
        /// </summary>
        /// <param name="measuresMember">Measures member.</param>
        public void Register(AdomdMeasuresMember measuresMember)
        {
            Assert.ArgumentNotNull(measuresMember, "measuresMember", "Measures member cannot be null.");
            Assert.ArgumentValid(!_measuresNamesDictionary.ContainsKey(measuresMember.UniqueName),
                "measuresMember", "Measure with the same name is already registered.");
            _measuresNamesDictionary[measuresMember.UniqueName] = measuresMember;
        }

        /// <summary>
        /// Updates ordinal positions for all tuples within the given CellSet.
        /// </summary>
        /// <param name="cellSet">CellSet contains tuples.</param>
        public void RegistersTuplesFromCellSet(Adomd.CellSet cellSet)
        {
            Assert.ArgumentNotNull(cellSet, "cellSet", "CellSet cannot be null.");
            _tupleOrdinalPositions.Clear();
            foreach (Adomd.Tuple sourceTuple in GetAllTuplesFromCellSet(cellSet))
            {
                Tuple modelTuple = ConvertAdomdToModelTuple(sourceTuple);
                _tupleOrdinalPositions.Add(modelTuple, sourceTuple.TupleOrdinal);
            }
        }

        private IEnumerable<Adomd.Tuple> GetAllTuplesFromCellSet(Adomd.CellSet cellSet)
        {
            foreach (Adomd.Axis sourceAxis in cellSet.Axes)
            {
                foreach (Adomd.Tuple sourceTuple in sourceAxis.Set.Tuples)
                {
                    yield return sourceTuple;
                }
            }
            if (cellSet.FilterAxis != null)
            {
                foreach (Adomd.Tuple sourceTuple in cellSet.FilterAxis.Set.Tuples)
                {
                    yield return sourceTuple;
                }
            }
        }

        private Tuple ConvertAdomdToModelTuple(Adomd.Tuple sourceTuple)
        {
            List<IMember> members = new List<IMember>();
            foreach (Adomd.Member sourceMember in sourceTuple.Members)
            {
                IMember modelMember;
                if (_measuresNamesDictionary.ContainsKey(sourceMember.UniqueName))
                {
                    modelMember = _measuresNamesDictionary[sourceMember.UniqueName];
                }
                else
                {
                    modelMember = GetMember(sourceMember);
                }
                members.Add(modelMember);
            }
            return new Tuple(members);
        }
        #endregion

        #region Getting model elements for source elements
        /// <summary>
        /// Gets model level associated with the given Adomd.NET level.
        /// </summary>
        /// <param name="sourceLevel">An Adomd.NET level.</param>
        /// <returns>Model level associated with the given Adomd.NET level.</returns>
        public AdomdLevel GetLevel(Adomd.Level sourceLevel)
        {
            return (sourceLevel != null) && _levelsDictionary.ContainsKey(sourceLevel) ?
                _levelsDictionary[sourceLevel] : null;
        }

        /// <summary>
        /// Get model member associated with the given Adomd.NET member.
        /// </summary>
        /// <param name="sourceMember">An Adomd.NET member.</param>
        /// <returns>Model member associated with the given Adomd.NET member.</returns>
        /// <remarks>
        /// This method creates the AdomdMember for given source Member if it doesn't exist.
        /// It also asumes that all levels are created before any member.
        /// </remarks>
        public AdomdMember GetMember(Adomd.Member sourceMember)
        {
            if (sourceMember == null)
            {
                return null;
            }

            AdomdMember result;
            if (!_membersDictionary.ContainsKey(sourceMember))
            {
                AdomdLevel level = GetLevel(sourceMember.ParentLevel);
                // The member will register itself within the registry.
                result = new AdomdMember(sourceMember, level, this);
            }
            else
            {
                result = _membersDictionary[sourceMember];
            }
            return result;           
        }

        /// <summary>
        /// Gets tuple's ordinal position.
        /// </summary>
        /// <param name="tuple">A tuple.</param>
        /// <returns>If the tuple was registered in the repository, returns its ordinal
        /// position; otherwise, returns -1.</returns>
        public int GetTupleOrdinalPosition(Tuple tuple)
        {
            Assert.ArgumentNotNull(tuple, "tuple", "Cannot return ordinal position of null tuple.");
            if (!_tupleOrdinalPositions.ContainsKey(tuple))
            {
                return -1;
            }
            return _tupleOrdinalPositions[tuple];
        }
        #endregion
    }
}
