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
                LightColour.Red,
                LightColour.Red.MergeWith(LightColour.Red)
            );
        }

        [Test]
        public void GreenMergeIdentity() {
            Assert.AreEqual(
                LightColour.Green,
                LightColour.Green.MergeWith(LightColour.Green)
            );
        }

        [Test]
        public void BlueMergeIdentity() {
            Assert.AreEqual(
                LightColour.Blue,
                LightColour.Blue.MergeWith(LightColour.Blue)
            );
        }

        [Test]
        public void MagentaMergeIdentity() {
            Assert.AreEqual(
                LightColour.Magenta,
                LightColour.Magenta.MergeWith(LightColour.Magenta)
            );
        }

        [Test]
        public void CyanMergeIdentity() {
            Assert.AreEqual(
                LightColour.Cyan,
                LightColour.Cyan.MergeWith(LightColour.Cyan)
            );
        }

        [Test]
        public void YellowMergeIdentity() {
            Assert.AreEqual(
                LightColour.Yellow,
                LightColour.Yellow.MergeWith(LightColour.Yellow)
            );
        }

        [Test]
        public void RedMergeGreen() {
            Assert.AreEqual(
                LightColour.Yellow,
                LightColour.Red.MergeWith(LightColour.Green)
            );
        }

        [Test]
        public void RedMergeBlue() {
            Assert.AreEqual(
                LightColour.Magenta,
                LightColour.Red.MergeWith(LightColour.Blue)
            );
        }

        [Test]
        public void GreenMergeRed() {
            Assert.AreEqual(
                LightColour.Yellow,
                LightColour.Green.MergeWith(LightColour.Red)
            );
        }

        [Test]
        public void GreenMergeBlue() {
            Assert.AreEqual(
                LightColour.Cyan,
                LightColour.Green.MergeWith(LightColour.Blue)
            );
        }

        [Test]
        public void BlueMergeRed() {
            Assert.AreEqual(
                LightColour.Magenta,
                LightColour.Blue.MergeWith(LightColour.Red)
            );
        }

        [Test]
        public void BlueMergeGreen() {
            Assert.AreEqual(
                LightColour.Cyan,
                LightColour.Blue.MergeWith(LightColour.Green)
            );
        }

        [Test]
        public void MagentaMergeRed() {
            Assert.AreEqual(
                LightColour.Magenta,
                LightColour.Magenta.MergeWith(LightColour.Red)
            );
        }

        [Test]
        public void MagentaMergeGreen() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.Magenta.MergeWith(LightColour.Green)
            );
        }

        [Test]
        public void MagentaMergeBlue() {
            Assert.AreEqual(
                LightColour.Magenta,
                LightColour.Magenta.MergeWith(LightColour.Blue)
            );
        }

        [Test]
        public void MagentaMergeCyan() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.Magenta.MergeWith(LightColour.Cyan)
            );
        }

        [Test]
        public void MagentaMergeYellow() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.Magenta.MergeWith(LightColour.Yellow)
            );
        }

        [Test]
        public void CyanMergeRed() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.Cyan.MergeWith(LightColour.Red)
            );
        }
 
        [Test]
        public void CyanMergeGreen() {
            Assert.AreEqual(
                LightColour.Cyan,
                LightColour.Cyan.MergeWith(LightColour.Green)
            );
        }
 
        [Test]
        public void CyanMergeBlue() {
            Assert.AreEqual(
                LightColour.Cyan,
                LightColour.Cyan.MergeWith(LightColour.Blue)
            );
        }

        [Test]
        public void CyanMergeYellow() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.Cyan.MergeWith(LightColour.Yellow)
            );
        }

        [Test]
        public void CyanMergeMagenta() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.Cyan.MergeWith(LightColour.Magenta)
            );
        }
 
        [Test]
        public void YellowMergeRed() {
            Assert.AreEqual(
                LightColour.Yellow,
                LightColour.Yellow.MergeWith(LightColour.Red)
            );
        }
 
        [Test]
        public void YellowMergeGreen() {
            Assert.AreEqual(
                LightColour.Yellow,
                LightColour.Yellow.MergeWith(LightColour.Green)
            );
        }
 
        [Test]
        public void YellowMergeBlue() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.Yellow.MergeWith(LightColour.Blue)
            );
        }
 
        [Test]
        public void YellowMergeCyan() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.Yellow.MergeWith(LightColour.Cyan)
            );
        }
 
        [Test]
        public void YellowMergeMagenta() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.Yellow.MergeWith(LightColour.Magenta)
            );
        }

        [Test]
        public void WhiteMergeRed() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.White.MergeWith(LightColour.Red)
            );
        }

        [Test]
        public void WhiteMergeGreen() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.White.MergeWith(LightColour.Green)
            );
        }

        [Test]
        public void WhiteMergeBlue() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.White.MergeWith(LightColour.Blue)
            );
        }

        [Test]
        public void WhiteMergeCyan() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.White.MergeWith(LightColour.Cyan)
            );
        }

        [Test]
        public void WhiteMergeYellow() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.White.MergeWith(LightColour.Yellow)
            );
        }

        [Test]
        public void WhiteMergeMagenta() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.White.MergeWith(LightColour.Magenta)
            );
        }

        [Test]
        public void WhiteMergeWhite() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.White.MergeWith(LightColour.White)
            );
        }

        [Test]
        public void BlackMergeBlack() {
            Assert.AreEqual(
                LightColour.Black,
                LightColour.Black.MergeWith(LightColour.Black)
            );
        }

        [Test]
        public void BlackMergeRed() {
            Assert.AreEqual(
                LightColour.Red,
                LightColour.Black.MergeWith(LightColour.Red)
            );
        }

        [Test]
        public void BlackMergeGreen() {
            Assert.AreEqual(
                LightColour.Green,
                LightColour.Black.MergeWith(LightColour.Green)
            );
        }

        [Test]
        public void BlackMergeBlue() {
            Assert.AreEqual(
                LightColour.Blue,
                LightColour.Black.MergeWith(LightColour.Blue)
            );
        }

        [Test]
        public void BlackMergeMagenta() {
            Assert.AreEqual(
                LightColour.Magenta,
                LightColour.Black.MergeWith(LightColour.Magenta)
            );
        }

        [Test]
        public void BlackMergeYellow() {
            Assert.AreEqual(
                LightColour.Yellow,
                LightColour.Black.MergeWith(LightColour.Yellow)
            );
        }

        [Test]
        public void BlackMergeCyan() {
            Assert.AreEqual(
                LightColour.Cyan,
                LightColour.Black.MergeWith(LightColour.Cyan)
            );
        }

        [Test]
        public void BlackMergeWhite() {
            Assert.AreEqual(
                LightColour.White,
                LightColour.Black.MergeWith(LightColour.White)
            );
        }
    }
}
