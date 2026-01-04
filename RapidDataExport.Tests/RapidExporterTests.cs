using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ControllerAPI;
using ABB.Robotics.Controllers.RapidDomain;

namespace RapidDataExport.Tests
{
    [TestClass]
    public class RapidExporterTests
    {
        [TestMethod]
        public void TryFormatValue_Num_ReturnsValue()
        {
            // Arrange
            var exporter = new RapidExporter();
            string valor;

            // Act
            bool ok = exporter.TryFormatValue("num", new Num(12.5), out valor);

            // Assert
            Assert.IsTrue(ok);
            Assert.AreEqual("12,5", valor);
        }
        [TestMethod]
        public void TryFormatValue_Dnum_ReturnsValue()
        {
            // Arrange
            var exporter = new RapidExporter();
            string valor;

            // Act
            bool ok = exporter.TryFormatValue("dnum", new Dnum(12.554), out valor);

            // Assert
            Assert.IsTrue(ok);
            Assert.AreEqual("12,554", valor);
        }
    }
}
