/*
  Hydra compiler - Symbol table class.
  Copyright (C) 2013-2020 Ariel Ortiz, ITESM CEM,
  Modified by: Gerardo Galván, Jesús Perea, Jorge López.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Hydra_compiler {
  public class GlobalVariables : IEnumerable<GlobalVariable> {

    List<GlobalVariable> data = new List<GlobalVariable> ();

    public GlobalVariable this [int index] {
      get {
        return data[index];
      }
      set {
        data[index] = value;
      }
    }

    public int Count {
      get {
        return data.Count;
      }
    }

    public void Add (GlobalVariable element) {
      data.Add (element);
    }

    public bool Contains (string key) {
      foreach (var item in data) {
        if (item.name == key) return true;
      }
      return false;
    }

    public override string ToString () {
      var sb = new StringBuilder ();
      sb.Append ("Global Variables\n");
      sb.Append ("=======\n");
      foreach (var entry in data) {
        sb.Append ($"{entry}\n");
      }
      sb.Append ("=======\n");
      return sb.ToString ();
    }

    public IEnumerator<GlobalVariable> GetEnumerator () {
      return data.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator () {
      throw new NotImplementedException ();
    }
  }

  public class GlobalVariable {
    public string name;
    public dynamic value;

    public GlobalVariable (string name, dynamic value) {
      this.name = name;
      this.value = value;
    }
  }

  public class GlobalFunctions : IEnumerable<KeyValuePair<string, FunctionData>> {

    IDictionary<string, FunctionData> data = new SortedDictionary<string, FunctionData> ();

    public override string ToString () {
      var sb = new StringBuilder ();
      sb.Append ("Global Function Table\n");
      sb.Append ("====================\n");
      foreach (var entry in data) {
        sb.Append ($"{entry.Key}: {entry.Value}\n");
      }
      sb.Append ("====================\n");
      return sb.ToString ();
    }

    //-----------------------------------------------------------
    public FunctionData this [string key] {
      get {
        return data[key];
      }
      set {
        data[key] = value;
      }
    }

    //-----------------------------------------------------------
    public bool Contains (string key) {
      return data.ContainsKey (key);
    }

    public IEnumerator<KeyValuePair<string, FunctionData>> GetEnumerator () {
      return data.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator () {
      throw new NotImplementedException ();
    }
  }

  public class FunctionData {
    public bool isPrimitive;
    public int arity;
    public LocalSymbolTable RefST;

    public override string ToString () {
      var sb = new StringBuilder ();
      string hasRefSTString = this.RefST == null ? "   |   Local Symbol Table" : "";
      sb.Append ($"\n\n\tIsPrimitive   |   Arity{hasRefSTString}\n");
      sb.Append ("\t========================\n");
      string hasRefSTValue = this.RefST == null ? "\t      |     null" : "";
      sb.Append ($"\t{this.isPrimitive}\t      |     {this.arity}{hasRefSTValue}\n");
      if (this.RefST != null) {
        sb.Append ("\t------------------------\n\n");
        sb.Append ($"{this.RefST}");
      }
      sb.Append ("\t========================\n");
      return sb.ToString ();
    }
  }

  public class LocalSymbolTable : IEnumerable<KeyValuePair<string, bool>> {

    public LocalSymbolTable (string name) {
      this.name = name;
    }
    IDictionary<string, bool> data = new SortedDictionary<string, bool> ();
    public string name;

    public IEnumerator<KeyValuePair<string, bool>> GetEnumerator () {
      return data.GetEnumerator ();
    }

    public override string ToString () {
      var sb = new StringBuilder ();
      sb.Append ("\tLocal Symbol Table\n");
      sb.Append ("\t====================\n");
      sb.Append ("\tName  |  Is Param\n");
      sb.Append ("\t====================\n");
      foreach (var entry in data) {
        sb.Append ($"\t{entry.Key}     |    {entry.Value}\n");
      }
      sb.Append ("\t====================\n");
      return sb.ToString ();
    }

    //-----------------------------------------------------------
    public bool this [string key] {
      get {
        return data[key];
      }
      set {
        data[key] = value;
      }
    }
    //-----------------------------------------------------------
    public bool Contains (string key) {
      return data.ContainsKey (key);
    }

    IEnumerator IEnumerable.GetEnumerator () {
      throw new NotImplementedException ();
    }
  }
}