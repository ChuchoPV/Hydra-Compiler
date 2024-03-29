/*
  Hydra compiler - Node class for the Parser.
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
using System.Collections.Generic;
using System.Text;

namespace Hydra_compiler {

  public class Node : IEnumerable<Node> {

    IList<Node> children = new List<Node> ();

    public Node this [int index] {
      get {
        return children[index];
      }
    }

    public Token AnchorToken { get; set; }

    public void Add (Node node) {
      children.Add (node);
    }

    public int Count () {
      return children.Count;
    }

    public void Remove (int index) {
      children.RemoveAt(index);
    }

    public void RemoveNodes (int index) {
      for (var i = this.Count() - 1; i >= index; i--) {
        this.Remove (i);
      }
    }

    public int BreakIndex () {
      var i = 0;
      foreach (var child in this) {
        if (child is Break) {
          return i;
        }
        i++;
      }
      return -1;
    }

    public int ReturnIndex () {
      var i = 0;
      foreach (var child in this) {
        if (child is Return) {
          return i;
        }
        i++;
      }
      return -1;
    }

    public IEnumerator<Node> GetEnumerator () {
      return children.GetEnumerator ();
    }

    System.Collections.IEnumerator
    System.Collections.IEnumerable.GetEnumerator () {
      return children.GetEnumerator();
    }

    public override string ToString () {
      return $"{GetType().Name} {AnchorToken}";
    }

    public string ToStringTree () {
      var sb = new StringBuilder ();
      TreeTraversal (this, "", sb);
      return sb.ToString ();
    }

    static void TreeTraversal (Node node, string indent, StringBuilder sb) {
      sb.Append (indent);
      sb.Append (node);
      sb.Append ('\n');
      foreach (var child in node.children) {
        TreeTraversal (child, indent + "  ", sb);
      }
    }
  }
}