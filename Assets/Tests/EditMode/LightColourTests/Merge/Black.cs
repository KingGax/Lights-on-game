using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightColourTests {
namespace MergeWith {
public class Black {

    [Test]
    public void MergeWithBlack() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Black.MergeWith(LightColour.Black)
        );
    }

    [Test]
    public void MergeWithRed() {
        Assert.AreEqual(
            LightColour.Red,
            LightColour.Black.MergeWith(LightColour.Red)
        );
    }

    [Test]
    public void MergeWithGreen() {
        Assert.AreEqual(
            LightColour.Green,
            LightColour.Black.MergeWith(LightColour.Green)
        );
    }

    [Test]
    public void MergeWithBlue() {
        Assert.AreEqual(
            LightColour.Blue,
            LightColour.Black.MergeWith(LightColour.Blue)
        );
    }

    [Test]
    public void MergeWithCyan() {
        Assert.AreEqual(
            LightColour.Cyan,
            LightColour.Black.MergeWith(LightColour.Cyan)
        );
    }

    [Test]
    public void MergeWithYellow() {
        Assert.AreEqual(
            LightColour.Yellow,
            LightColour.Black.MergeWith(LightColour.Yellow)
        );
    }

    [Test]
    public void MergeWithMagenta() {
        Assert.AreEqual(
            LightColour.Magenta,
            LightColour.Black.MergeWith(LightColour.Magenta)
        );
    }

    [Test]
    public void MergeWithWhite() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.Black.MergeWith(LightColour.White)
        );
    }
} } }