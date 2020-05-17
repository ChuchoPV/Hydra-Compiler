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
  public class GlobalVariables : IEnumerable<string> {

    List<string> data = new List<string> ();

    public string this [int index] {
      get {
        return data[index];
      }
      set {
        data[index] = value;
      }
    }

    public void Add (string element) {
      data.Add (element);
    }

    public bool Contains (string key) {
      return data.Contains (key);
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

    public IEnumerator<string> GetEnumerator () {
      return data.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator () {
      throw new NotImplementedException ();
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
      sb.Append ("\tIsPrimitive   |   Arity    |   Symbol Table\n");
      sb.Append ("\t=========================================\n");
      sb.Append ($"\t{this.isPrimitive}\t|\t{this.arity}\t|\t{this.RefST}\n");
      sb.Append ("\t=========================================\n");
      return sb.ToString ();
    }
  }

  public class LocalSymbolTable : IEnumerable<KeyValuePair<string, bool>> {

    IDictionary<string, bool> data = new SortedDictionary<string, bool> ();

    public IEnumerator<KeyValuePair<string, bool>> GetEnumerator () {
      return data.GetEnumerator ();
    }

    public override string ToString () {
      var sb = new StringBuilder ();
      sb.Append ("Local Symbol Table\n");
      sb.Append (@"Name    |   Is Param");
      sb.Append ("====================\n");
      foreach (var entry in data) {
        sb.Append ($"{entry.Key}: {entry.Value}\n");
      }
      sb.Append ("====================\n");
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