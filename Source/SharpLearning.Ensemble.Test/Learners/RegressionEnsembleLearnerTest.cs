﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpLearning.Common.Interfaces;
using SharpLearning.DecisionTrees.Learners;
using SharpLearning.Ensemble.Learners;
using SharpLearning.Ensemble.Strategies;
using SharpLearning.Ensemble.Test.Properties;
using SharpLearning.InputOutput.Csv;
using SharpLearning.Metrics.Regression;
using System.IO;
using System.Linq;

namespace SharpLearning.Ensemble.Test.Learners
{
    [TestClass]
    public class RegressionEnsembleLearnerTest
    {
        [TestMethod]
        public void RegressionEnsembleLearner_Learn()
        {
            var learners = new IIndexedLearner<double>[]
            {
                new RegressionDecisionTreeLearner(2),
                new RegressionDecisionTreeLearner(5),
                new RegressionDecisionTreeLearner(7),
                new RegressionDecisionTreeLearner(9)
            };

            var sut = new RegressionEnsembleLearner(learners, new MeanRegressionEnsembleStrategy());

            var parser = new CsvParser(() => new StringReader(Resources.DecisionTreeData));
            var observations = parser.EnumerateRows("F1", "F2").ToF64Matrix();
            var targets = parser.EnumerateRows("T").ToF64Vector();

            var model = sut.Learn(observations, targets);
            var predictions = model.Predict(observations);

            var evaluator = new MeanSquaredErrorRegressionMetric();
            var actual = evaluator.Error(targets, predictions);

            Assert.AreEqual(0.021845326201904366, actual, 0.0001);
        }

        [TestMethod]
        public void RegressionEnsembleLearner_Learn_Bagging()
        {
            var learners = new IIndexedLearner<double>[]
            {
                new RegressionDecisionTreeLearner(2),
                new RegressionDecisionTreeLearner(5),
                new RegressionDecisionTreeLearner(7),
                new RegressionDecisionTreeLearner(9)
            };

            var sut = new RegressionEnsembleLearner(learners, new MeanRegressionEnsembleStrategy(), 0.7);

            var parser = new CsvParser(() => new StringReader(Resources.DecisionTreeData));
            var observations = parser.EnumerateRows("F1", "F2").ToF64Matrix();
            var targets = parser.EnumerateRows("T").ToF64Vector();

            var model = sut.Learn(observations, targets);
            var predictions = model.Predict(observations);

            var evaluator = new MeanSquaredErrorRegressionMetric();
            var actual = evaluator.Error(targets, predictions);

            Assert.AreEqual(0.0318163057544871, actual, 0.0001);
        }

        [TestMethod]
        public void RegressionEnsembleLearner_Learn_Indexed()
        {
            var learners = new IIndexedLearner<double>[]
            {
                new RegressionDecisionTreeLearner(2),
                new RegressionDecisionTreeLearner(5),
                new RegressionDecisionTreeLearner(7),
                new RegressionDecisionTreeLearner(9)
            };

            var sut = new RegressionEnsembleLearner(learners, new MeanRegressionEnsembleStrategy());

            var parser = new CsvParser(() => new StringReader(Resources.DecisionTreeData));
            var observations = parser.EnumerateRows("F1", "F2").ToF64Matrix();
            var targets = parser.EnumerateRows("T").ToF64Vector();
            var indices = Enumerable.Range(0, 25).ToArray();

            var model = sut.Learn(observations, targets, indices);
            var predictions = model.Predict(observations);

            var evaluator = new MeanSquaredErrorRegressionMetric();
            var actual = evaluator.Error(targets, predictions);

            Assert.AreEqual(0.13909386278812202, actual, 0.0001);
        }
    }
}
