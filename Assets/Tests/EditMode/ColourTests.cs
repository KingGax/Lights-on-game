using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightingTests {
    public class ColourTests {

        [Test]
        public void RedMergeIdentity() {
            Assert.AreEqual(
                LightableColour.Red,
                LightableColour.Red.MergeWith(LightableColour.Red)
            );
        }

        [Test]
        public void GreenMergeIdentity() {
            Assert.AreEqual(
                LightableColour.Green,
                LightableColour.Green.MergeWith(LightableColour.Green)
            );
        }

        [Test]
        public void BlueMergeIdentity() {
            Assert.AreEqual(
                LightableColour.Blue,
                LightableColour.Blue.MergeWith(LightableColour.Blue)
            );
        }

        [Test]
        public void MagentaMergeIdentity() {
            Assert.AreEqual(
                LightableColour.Magenta,
                LightableColour.Magenta.MergeWith(LightableColour.Magenta)
            );
        }

        [Test]
        public void CyanMergeIdentity() {
            Assert.AreEqual(
                LightableColour.Cyan,
                LightableColour.Cyan.MergeWith(LightableColour.Cyan)
            );
        }

        [Test]
        public void YellowMergeIdentity() {
            Assert.AreEqual(
                LightableColour.Yellow,
                LightableColour.Yellow.MergeWith(LightableColour.Yellow)
            );
        }

        [Test]
        public void RedMergeGreen() {
            Assert.AreEqual(
                LightableColour.Yellow,
                LightableColour.Red.MergeWith(LightableColour.Green)
            );
        }

        [Test]
        public void RedMergeBlue() {
            Assert.AreEqual(
                LightableColour.Magenta,
                LightableColour.Red.MergeWith(LightableColour.Blue)
            );
        }

        [Test]
        public void GreenMergeRed() {
            Assert.AreEqual(
                LightableColour.Yellow,
                LightableColour.Green.MergeWith(LightableColour.Red)
            );
        }

        [Test]
        public void GreenMergeBlue() {
            Assert.AreEqual(
                LightableColour.Cyan,
                LightableColour.Green.MergeWith(LightableColour.Blue)
            );
        }

        [Test]
        public void BlueMergeRed() {
            Assert.AreEqual(
                LightableColour.Magenta,
                LightableColour.Blue.MergeWith(LightableColour.Red)
            );
        }

        [Test]
        public void BlueMergeGreen() {
            Assert.AreEqual(
                LightableColour.Cyan,
                LightableColour.Blue.MergeWith(LightableColour.Green)
            );
        }

        [Test]
        public void MagentaMergeRed() {
            Assert.AreEqual(
                LightableColour.Magenta,
                LightableColour.Magenta.MergeWith(LightableColour.Red)
            );
        }

        [Test]
        public void MagentaMergeGreen() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.Magenta.MergeWith(LightableColour.Green)
            );
        }

        [Test]
        public void MagentaMergeBlue() {
            Assert.AreEqual(
                LightableColour.Magenta,
                LightableColour.Magenta.MergeWith(LightableColour.Blue)
            );
        }

        [Test]
        public void MagentaMergeCyan() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.Magenta.MergeWith(LightableColour.Cyan)
            );
        }

        [Test]
        public void MagentaMergeYellow() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.Magenta.MergeWith(LightableColour.Yellow)
            );
        }

        [Test]
        public void CyanMergeRed() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.Cyan.MergeWith(LightableColour.Red)
            );
        }
 
        [Test]
        public void CyanMergeGreen() {
            Assert.AreEqual(
                LightableColour.Cyan,
                LightableColour.Cyan.MergeWith(LightableColour.Green)
            );
        }
 
        [Test]
        public void CyanMergeBlue() {
            Assert.AreEqual(
                LightableColour.Cyan,
                LightableColour.Cyan.MergeWith(LightableColour.Blue)
            );
        }

        [Test]
        public void CyanMergeYellow() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.Cyan.MergeWith(LightableColour.Yellow)
            );
        }

        [Test]
        public void CyanMergeMagenta() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.Cyan.MergeWith(LightableColour.Magenta)
            );
        }
 
        [Test]
        public void YellowMergeRed() {
            Assert.AreEqual(
                LightableColour.Yellow,
                LightableColour.Yellow.MergeWith(LightableColour.Red)
            );
        }
 
        [Test]
        public void YellowMergeGreen() {
            Assert.AreEqual(
                LightableColour.Yellow,
                LightableColour.Yellow.MergeWith(LightableColour.Green)
            );
        }
 
        [Test]
        public void YellowMergeBlue() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.Yellow.MergeWith(LightableColour.Blue)
            );
        }
 
        [Test]
        public void YellowMergeCyan() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.Yellow.MergeWith(LightableColour.Cyan)
            );
        }
 
        [Test]
        public void YellowMergeMagenta() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.Yellow.MergeWith(LightableColour.Magenta)
            );
        }

        [Test]
        public void WhiteMergeRed() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.White.MergeWith(LightableColour.Red)
            );
        }

        [Test]
        public void WhiteMergeGreen() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.White.MergeWith(LightableColour.Green)
            );
        }

        [Test]
        public void WhiteMergeBlue() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.White.MergeWith(LightableColour.Blue)
            );
        }

        [Test]
        public void WhiteMergeCyan() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.White.MergeWith(LightableColour.Cyan)
            );
        }

        [Test]
        public void WhiteMergeYellow() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.White.MergeWith(LightableColour.Yellow)
            );
        }

        [Test]
        public void WhiteMergeMagenta() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.White.MergeWith(LightableColour.Magenta)
            );
        }

        [Test]
        public void WhiteMergeWhite() {
            Assert.AreEqual(
                LightableColour.White,
                LightableColour.White.MergeWith(LightableColour.White)
            );
        }
    }
}
