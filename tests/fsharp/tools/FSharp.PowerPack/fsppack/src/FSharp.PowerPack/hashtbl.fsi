//==========================================================================
// (c) Microsoft Corporation 2005-2008.   The interface to the module 
// is similar to that found in versions of other ML implementations, 
// but is not an exact match.  The type signatures in this interface
// are an edited version of those generated automatically by running 
// "bin\fsc.exe -i" on the implementation file.
//===========================================================================


/// Multi-entry hash tables using the structural "hash" and "equals" functions.  
///
///These tables can be used with keys of any type, but you should check that
///structural hashing and equality are correct for your key type.  
///Structural hashing is efficient but not a suitable choice in all circumstances, 
///e.g. may not hash efficiently on non-reference types and deeply-structured types.
///Better efficiency is typically achieved if key types are F#-generated
///types.
///
///These hash tables may map items to multiple keys (see find_all).
///
///The implementations are not safe for concurrent reading/writing,
///and so users of these tables should take an appropriate lock
///before reading/writing if used in a concurrent setting.
[<CompilerMessage("This module is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Microsoft.FSharp.Compatibility.OCaml.Hashtbl
open Microsoft.FSharp.Collections

open System
open System.IO
open System.Collections.Generic


/// OCaml compatible type name, for use when not opening module, e.g. Hashtbl.t
type t<'Key,'Value> = HashMultiMap<'Key,'Value> 

/// Create a hash table with the suggested initial size.  
///
/// Inlined to enable generation of efficient hash routines for the key type in the common case.
val inline create  : int -> HashMultiMap<'Key,'Value> when 'Key : equality

/// Add key and data to the table.
val add     : HashMultiMap<'Key,'Value> -> 'Key -> 'Value -> unit when 'Key : equality

/// Create a hash table using the given data
val of_list : ('Key * 'Value) list -> HashMultiMap<'Key,'Value>  when 'Key : equality

/// Create hash table using the given data
val of_seq : seq<'Key * 'Value>  -> HashMultiMap<'Key,'Value>  when 'Key : equality

/// Empty the table.
val clear   : HashMultiMap<'Key,'Value> -> unit

/// Create a copy of the table. Remember they are imperative and get mutated. 
val copy    : HashMultiMap<'Key,'Value> -> HashMultiMap<'Key,'Value>

/// Lookup key's data in the table.
/// Raises exception is key not in table, if this could happen you should be using tryFind.
val find    : HashMultiMap<'Key,'Value> -> 'Key -> 'Value

/// Return all bindings for the given key
val find_all: HashMultiMap<'Key,'Value> -> 'Key -> 'Value list

/// Fold over all bindings
val fold    : ('Key -> 'Value -> 'State -> 'State) -> HashMultiMap<'Key,'Value> -> 'State -> 'State

///Apply the given function to each binding in the hash table 
val iter    : ('Key -> 'Value -> unit) -> HashMultiMap<'Key,'Value> -> unit

/// Test for the existence of any bindings for the given key
val mem    : HashMultiMap<'Key,'Value> -> 'Key -> bool

/// Remove the latest binding for the given key
val remove : HashMultiMap<'Key,'Value> -> 'Key -> unit

/// Replace the latest binding for the given key
val replace: HashMultiMap<'Key,'Value> -> 'Key -> 'Value -> unit

/// Lookup the key's data in the table
val tryfind: HashMultiMap<'Key,'Value> -> 'Key -> 'Value option

/// Hash on the structure of a value according to the F# structural hashing
/// conventions
val hash : 'T -> int 

/// Hash on the identity of an object. 
val hashq: 'T -> int   when 'T : not struct

///A collection of operations for creating and using hash tables based on particular type-tracked hash/equality functions.
///Generated by the Hashtbl.Make and Hashtbl.MakeTagged functors. This type is for use when you wish to
///specify a comparison function once and carry around an object that is a provider of (i.e. a factory for) hashtables 
///that utilize that comparison function.
///
///The 'Tag' type parameter is used to track information about the comparison function, which helps ensure 
///that you don't mixup maps created with different comparison functions
type Provider<'Key,'Value,'Tag>  
     when 'Tag :> IEqualityComparer<'Key> =
  interface
    abstract create  : int -> Tagged.HashMultiMap<'Key,'Value,'Tag>;
    abstract clear   : Tagged.HashMultiMap<'Key,'Value,'Tag> -> unit;
    abstract add     : Tagged.HashMultiMap<'Key,'Value,'Tag> -> 'Key -> 'Value -> unit;
    abstract copy    : Tagged.HashMultiMap<'Key,'Value,'Tag> -> Tagged.HashMultiMap<'Key,'Value,'Tag>;
    abstract find    : Tagged.HashMultiMap<'Key,'Value,'Tag> -> 'Key -> 'Value;
    abstract find_all: Tagged.HashMultiMap<'Key,'Value,'Tag> -> 'Key -> 'Value list;
    abstract tryfind : Tagged.HashMultiMap<'Key,'Value,'Tag> -> 'Key -> 'Value option;
    abstract mem     : Tagged.HashMultiMap<'Key,'Value,'Tag> -> 'Key -> bool;
    abstract remove  : Tagged.HashMultiMap<'Key,'Value,'Tag> -> 'Key -> unit;
    abstract replace : Tagged.HashMultiMap<'Key,'Value,'Tag> -> 'Key -> 'Value -> unit;
    abstract iter    : ('Key -> 'Value -> unit) -> Tagged.HashMultiMap<'Key,'Value,'Tag> -> unit;
    abstract fold    : ('Key -> 'Value -> 'State -> 'State) -> Tagged.HashMultiMap<'Key,'Value,'Tag> -> 'State -> 'State;
  end

type Provider<'Key,'Value> = Provider<'Key,'Value,IEqualityComparer<'Key>>

/// Same as Make, except track the comparison function being used through an additional type parameter.
///
/// To use this function accurately you need to define a new named class that implements IEqualityComparer and
/// pass an instance of that class as the first argument. For example:
///      type MyHasher = 
///        class
///          new() = { }
///          interface IEqualityComparer<string> with 
///            member self.GetHashCode(x) = ...
///            member self.Equals(x,y) = ...
///          end
///        end
///
/// let MyStringHashProvider : Hashtbl.Provider<string,int> = Hashtbl.MakeTagged(new MyStringHasher())
val MakeTagged: ('Tag :> IEqualityComparer<'Key>) -> Provider<'Key,'Value,'Tag>

/// Build a collection of operations for creating and using 
/// hashtables based on the given hash/equality functions. This returns a record
/// that contains the functions you use to create and manipulate tables of
/// this kind.  The returned value is much like an ML module. You should call Make once for 
/// each new pair of key/value types.  You may need to constrain the result 
/// to be an instantiation of Provider.
///
/// let MyStringHashProvider : Provider<string,int> = Hashtbl.Make(myStringHash,myStringEq)
val Make: ('Key -> int) * ('Key -> 'Key -> bool) -> Provider<'Key,'Value>
