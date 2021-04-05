using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightColourTests {
namespace MergeWith {
public class Blue {

    [Test]
    public void MergeWithBlack() {
        Assert.AreEqual(
            LightColour.Blue,
            LightColour.Blue.MergeWith(LightColour.Black)
        );
    }

    [Test]
    public void MergeWithRed() {
        Assert.AreEqual(
            LightColour.Magenta,
            LightColour.Blue.MergeWith(LightColour.Red)
        );
    }

    [Test]
    public void MergeWithGreen() {
        Assert.AreEqual(
            LightColour.Cyan,
            LightColour.Blue.MergeWith(LightColour.Green)
        );
    }

    [Test]
    public void MergeWithBlue() {
        Assert.AreEqual(
            LightColour.Blue,
            LightColour.Blue.MergeWith(LightColour.Blue)
        );
    }

    [Test]
    public void MergeWithCyan() {
        Assert.AreEqual(
            LightColour.Cyan,
            LightColour.Blue.MergeWith(LightColour.Cyan)
        );
    }

    [Test]
    public void MergeWithYellow() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.Blue.MergeWith(LightColour.Yellow)
        );
    }

    [Test]
    public void MergeWithMagenta() {
        Assert.AreEqual(
            LightColour.Magenta,
            LightColour.Blue.MergeWith(LightColour.Magenta)
        );
    }

    [Test]
    public void MergeWithWhite() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.Blue.MergeWith(LightColour.White)
        );
    }
} } }