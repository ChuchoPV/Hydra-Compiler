;; WebAssembly text format code generated by the hydra compiler.

(module
  (import "hydra" "new" (func $new (param i32) (result i32)))
  (import "hydra" "add" (func $add (param i32) (param i32) (result i32)))
  (import "hydra" "prints" (func $prints (param i32) (result i32)))
  (import "hydra" "readi" (func $readi (result i32)))
  (import "hydra" "printi" (func $printi (param i32) (result i32)))
  (import "hydra" "println" (func $println (result i32)))
  (import "hydra" "reads" (func $reads (result i32)))
  (import "hydra" "size" (func $size (param i32) (result i32)))
  (import "hydra" "get" (func $get (param i32) (param i32) (result i32)))
  (func $iterative_factorial (param $n i32) 
		(result i32)
    (local $result i32)
    (local $i i32)
    i32.const 1
		local.set $result
    i32.const 2
		local.set $i
		block $00000
				loop $00001
		local.get $i
		local.get $n
    i32.le_s
				i32.eqz
				br_if $00000
		local.get $result
		local.get $i
    i32.mul
		local.set $result
		i32.const 1
		local.get $i
		i32.add
		local.set $i
				br $00001
				end
		end
		local.get $result
		return
		i32.const 0
	)
  (func $recursive_factorial (param $n i32) 
		(result i32)
		local.get $n
    i32.const 0
    i32.le_s
		if
    i32.const 1
		return
		else
		local.get $n
		local.get $n
    i32.const 1
    i32.sub
		call $recursive_factorial
    i32.mul
		return
		end
		i32.const 0
	)
  (func (export "main") 
		(result i32)
		(local $00004 i32)
		(local $00005 i32)
		(local $00006 i32)
		(local $00007 i32)
    (local $num i32)
    (local $option i32)
				block $00002
						loop $00003
    i32.const 1
						i32.eqz
						br_if $00002
		i32.const 0
		call $new
		local.set $00004
		local.get $00004
    i32.const 73
		call $add
		drop
		local.get $00004
    i32.const 110
		call $add
		drop
		local.get $00004
    i32.const 112
		call $add
		drop
		local.get $00004
    i32.const 117
		call $add
		drop
		local.get $00004
    i32.const 116
		call $add
		drop
		local.get $00004
    i32.const 32
		call $add
		drop
		local.get $00004
    i32.const 97
		call $add
		drop
		local.get $00004
    i32.const 32
		call $add
		drop
		local.get $00004
    i32.const 110
		call $add
		drop
		local.get $00004
    i32.const 117
		call $add
		drop
		local.get $00004
    i32.const 109
		call $add
		drop
		local.get $00004
    i32.const 98
		call $add
		drop
		local.get $00004
    i32.const 101
		call $add
		drop
		local.get $00004
    i32.const 114
		call $add
		drop
		local.get $00004
    i32.const 58
		call $add
		drop
		local.get $00004
    i32.const 32
		call $add
		drop
		local.get $00004
		call $prints
		drop
		call $readi
		local.set $num
		i32.const 0
		call $new
		local.set $00005
		local.get $00005
    i32.const 73
		call $add
		drop
		local.get $00005
    i32.const 116
		call $add
		drop
		local.get $00005
    i32.const 101
		call $add
		drop
		local.get $00005
    i32.const 114
		call $add
		drop
		local.get $00005
    i32.const 97
		call $add
		drop
		local.get $00005
    i32.const 116
		call $add
		drop
		local.get $00005
    i32.const 105
		call $add
		drop
		local.get $00005
    i32.const 118
		call $add
		drop
		local.get $00005
    i32.const 101
		call $add
		drop
		local.get $00005
    i32.const 32
		call $add
		drop
		local.get $00005
    i32.const 102
		call $add
		drop
		local.get $00005
    i32.const 97
		call $add
		drop
		local.get $00005
    i32.const 99
		call $add
		drop
		local.get $00005
    i32.const 116
		call $add
		drop
		local.get $00005
    i32.const 111
		call $add
		drop
		local.get $00005
    i32.const 114
		call $add
		drop
		local.get $00005
    i32.const 105
		call $add
		drop
		local.get $00005
    i32.const 97
		call $add
		drop
		local.get $00005
    i32.const 108
		call $add
		drop
		local.get $00005
    i32.const 58
		call $add
		drop
		local.get $00005
    i32.const 32
		call $add
		drop
		local.get $00005
		call $prints
		drop
		local.get $num
		call $iterative_factorial
		call $printi
		drop
		call $println
		drop
		i32.const 0
		call $new
		local.set $00006
		local.get $00006
    i32.const 82
		call $add
		drop
		local.get $00006
    i32.const 101
		call $add
		drop
		local.get $00006
    i32.const 99
		call $add
		drop
		local.get $00006
    i32.const 117
		call $add
		drop
		local.get $00006
    i32.const 114
		call $add
		drop
		local.get $00006
    i32.const 115
		call $add
		drop
		local.get $00006
    i32.const 105
		call $add
		drop
		local.get $00006
    i32.const 118
		call $add
		drop
		local.get $00006
    i32.const 101
		call $add
		drop
		local.get $00006
    i32.const 32
		call $add
		drop
		local.get $00006
    i32.const 102
		call $add
		drop
		local.get $00006
    i32.const 97
		call $add
		drop
		local.get $00006
    i32.const 99
		call $add
		drop
		local.get $00006
    i32.const 116
		call $add
		drop
		local.get $00006
    i32.const 111
		call $add
		drop
		local.get $00006
    i32.const 114
		call $add
		drop
		local.get $00006
    i32.const 105
		call $add
		drop
		local.get $00006
    i32.const 97
		call $add
		drop
		local.get $00006
    i32.const 108
		call $add
		drop
		local.get $00006
    i32.const 58
		call $add
		drop
		local.get $00006
    i32.const 32
		call $add
		drop
		local.get $00006
		call $prints
		drop
		local.get $num
		call $recursive_factorial
		call $printi
		drop
		call $println
		drop
		i32.const 0
		call $new
		local.set $00007
		local.get $00007
    i32.const 67
		call $add
		drop
		local.get $00007
    i32.const 111
		call $add
		drop
		local.get $00007
    i32.const 109
		call $add
		drop
		local.get $00007
    i32.const 112
		call $add
		drop
		local.get $00007
    i32.const 117
		call $add
		drop
		local.get $00007
    i32.const 116
		call $add
		drop
		local.get $00007
    i32.const 101
		call $add
		drop
		local.get $00007
    i32.const 32
		call $add
		drop
		local.get $00007
    i32.const 97
		call $add
		drop
		local.get $00007
    i32.const 110
		call $add
		drop
		local.get $00007
    i32.const 111
		call $add
		drop
		local.get $00007
    i32.const 116
		call $add
		drop
		local.get $00007
    i32.const 104
		call $add
		drop
		local.get $00007
    i32.const 101
		call $add
		drop
		local.get $00007
    i32.const 114
		call $add
		drop
		local.get $00007
    i32.const 32
		call $add
		drop
		local.get $00007
    i32.const 102
		call $add
		drop
		local.get $00007
    i32.const 97
		call $add
		drop
		local.get $00007
    i32.const 99
		call $add
		drop
		local.get $00007
    i32.const 116
		call $add
		drop
		local.get $00007
    i32.const 111
		call $add
		drop
		local.get $00007
    i32.const 114
		call $add
		drop
		local.get $00007
    i32.const 105
		call $add
		drop
		local.get $00007
    i32.const 97
		call $add
		drop
		local.get $00007
    i32.const 108
		call $add
		drop
		local.get $00007
    i32.const 63
		call $add
		drop
		local.get $00007
    i32.const 32
		call $add
		drop
		local.get $00007
		call $prints
		drop
		call $reads
		local.set $option
		local.get $option
		call $size
    i32.const 0
    i32.eq
		if
    i32.const 78
		local.set $option
		else
		local.get $option
    i32.const 0
		call $get
		local.set $option
		end
		local.get $option
    i32.const 89
    i32.eq
		i32.eqz
		if (result i32)
		local.get $option
    i32.const 121
    i32.eq
		i32.eqz
		else
		i32.const 0
		end
		if
						br $00002
		else
		end
				br $00003
				end
		end
		i32.const 0
	)
)
