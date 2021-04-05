using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightColourTests {
namespace MergeWith {
public class White {

    [Test]
    public void MergeWithBlack() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.White.MergeWith(LightColour.Black)
        );
    }

    [Test]
    public void MergeWithRed() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.White.MergeWith(LightColour.Red)
        );
    }

    [Test]
    public void MergeWithGreen() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.White.MergeWith(LightColour.Green)
        );
    }

    [Test]
    public void MergeWithBlue() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.White.MergeWith(LightColour.Blue)
        );
    }

    [Test]
    public void MergeWithCyan() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.White.MergeWith(LightColour.Cyan)
        );
    }

    [Test]
    public void MergeWithYellow() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.White.MergeWith(LightColour.Yellow)
        );
    }

    [Test]
    public void MergeWithMagenta() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.White.MergeWith(LightColour.Magenta)
        );
    }

    [Test]
    public void MergeWithWhite() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.White.MergeWith(LightColour.White)
        );
    }
} } }