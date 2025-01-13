# C++ `std::unique_ptr<T>` for array type auto-delete

From the sounds of the Internet, this particular usage of std::unique_ptr is discouraged (feeding it an array type instead of a regular type).

I had a need for it anyway for a 2D array, *iff* is was working, as this would make some code not leak heap any more and remain easily readable (algorithm with array-indexing, so no std::vector overhead *please*).

Anyway, it turned out to be far easier to wrap the whole darn thing in a specialized storage class; see further below.

This is our test code to see if MSVC2022 would barf a hairball. It does, for the `typedef`-ed flavour in there that's been `#if 0`-ed:


```
class X1 {
public:
	X1(): v(42) {}
	X1(int i) : v(i) {}

	~X1() {
		v++;
	}

	int val() {
		return v;
	}

	int v;
};


// ...

	
	{
		std::unique_ptr<X1> p1(new X1(5));
		auto v1 = p1->v;
		auto v2 = p1->val();
	}
#if 0
	// on MSVC at least, unique_ptr for typedef'd array-types is b0rked: compiler errors ensue. It's strongly discouraged to use that sort of thing, anyway.
	{
		typedef X1 X2[100000000];

		// error C2664: 'std::unique_ptr<X2,std::default_delete<X2>>::unique_ptr(const std::unique_ptr<X2,std::default_delete<X2>> &)': cannot convert argument 1 from 'X1 *' to 'const std::unique_ptr<X2,std::default_delete<X2>> &'
		std::unique_ptr<X2> p1(new X2);
		auto v1 = p1[0].v;
		auto v2 = p1[0].val();
	}
#endif
	{
		std::unique_ptr<X1[]> p1(new X1[100000000]);
		auto v1 = p1[0].v;
		auto v2 = p1[0].val();
	}
```


And here's the 2D array class, ripped and stripped off std::array, which also has the problem of not accepting precalculated-at-runtime sizes, i.e. *variables* as template parameters: "*error C2971: 'array2D': template parameter 'row_size': 'l1': a variable with non-static storage duration cannot be used as a non-type argument*" and more of that ilk.

Which was the other reason why I came up with this bugger below.
The `_arr1Dfor2Daccessor_t` internal class construct is there to enable us to write userland code like: `d[i][0] = d[j][k] + 1`, i.e. nested `operator[]` invocations, so it all looks intuitive. See also the little sample code at the end of the next blurb, where this feature is used with wild abandon.

```
template <class T>
class array2D_dyn { // fixed, *dynamically sized*, array of values
public:
	using value_type      = T;
	using size_type       = size_t;
	using pointer         = T*;
	using const_pointer   = const T*;
	using reference       = T&;
	using const_reference = const T&;

	class _arr1Dfor2Daccessor_t {
	public:
		_arr1Dfor2Daccessor_t(pointer ref, pointer end):
			elems1D(ref), endptr(end)
		{}

		constexpr reference operator[](size_type col_pos) noexcept {
			if (elems1D + col_pos >= endptr)
				cerr << "kaboom!\n";
			return elems1D[col_pos];
		}

		constexpr const_reference operator[](size_type col_pos) const noexcept {
			if (elems1D + col_pos >= endptr)
				cerr << "kaboom!\n";
			return elems1D[col_pos];
		}

	protected:
		pointer elems1D;
		pointer endptr;
	};

	array2D_dyn(size_type _row_size, size_type _col_size):
		row_size(_row_size), col_size(_col_size)
	{
		auto count = size();
		elems = new T[count]();
		// https://isocpp.org/wiki/faq/exceptions#ctors-can-throw
		// --> throws std::bad_alloc exception when out of memory
		
		//assert(elems != nullptr);
	}

	~array2D_dyn()
	{
		delete[] elems;
	}

	constexpr size_type size() const noexcept {
		return row_size * col_size;
	}

	constexpr pointer data() const noexcept {
		return elems;
	}

	constexpr pointer end_of_data() const noexcept {
		return elems + size();
	}

	constexpr _arr1Dfor2Daccessor_t operator[](size_type row_pos) noexcept {
		const auto offset = row_pos * col_size;
		return _arr1Dfor2Daccessor_t(elems + offset, end_of_data());
	}

	constexpr const _arr1Dfor2Daccessor_t operator[](size_type row_pos) const noexcept {
		const auto offset = row_pos * col_size;
		return _arr1Dfor2Daccessor_t(elems + offset, end_of_data());
	}

	size_type row_size;
	size_type col_size;
	pointer elems;
};

// ....

int edit_distance_dp_basic(const string& str1, const string& str2)
{
	// allocate (no initialization)
	const auto l1 = str1.size();
	const auto l2 = str2.size();

	array2D_dyn<unsigned int> d(l1 + 1, l2 + 1);

	for (int i = 0; i <= l1; i++)
		d[i][0] = i;
    for (int i = 0; i <= l2; i++) 
		d[0][i] = i;
    for (int i = 1; i <= l1; i++)
        for (int j = 1; j <= l2; j++)
            d[i][j] = min(min(d[i-1][j], d[i][j-1]) + 1, d[i-1][j-1] + (str1[i-1] == str2[j-1] ? 0 : 1));

    auto rv = d[l1][l2];
	return rv;
} 

```

---

## Post Scriptum: minor bonus note: `std::unique_ptr(new Obj)` vs. `std::make_unique<Obj>()`

As seen on a C++ blog (link not immediately at hand while writing this from human memory, sorry) there's one minor performance improvement: `std::unique_ptr<X2> p1(new X2)` is not cache-optimal as it takes two(2) heap allocations: one for the `X2` object and one for the `std::unique_ptr` internals necessary for `unique_ptr` to deliver its gorgeous magic to us, which include a counter (for `weak_ptr` family relations) and a to-be-dereferenced *pointer* to the `X2` instance now located elsewhere on the heap. 
All this while the `std::make_unique` idiom only takes a *single* allocation, combining the `X2` instance and the `unique_ptr` internals into a single heap chunk, thus improving CPU access performance of the `X2` instance as the pointer dereference will very probably address (depending on the size of `X2` and the CPU cache line width) `X2` memory residing in the very same cache line, which is 👍 for performance.

The key there was that the `std::unique_ptr<X2> p1(new X2)` is only potentially beneficial when your usage of the `std::unique_ptr` is one where you use `.reset()`, `.swap()` or `operator=` to have it release its hold on your `X2` instance *early* and `free/delete` that one from the heap long before the `std::unique_ptr` instance itself goes out of scope, where its internals are released back into the heap: as `std::make_unique` treats both as a single heap chunk, your `X2` destructor may be called as early as before, but the actual releasing-back-allocated-memory-to-the-heap activity will have to wait until the `std::unique_ptr` instance goes out of scope as well, potentially increasing peak heap memory usage to undesirable levels then as the allocate-release timeline for the heap will be quite different. Otherwise, they argued (and I don't see any flaw in their logic), it's advisable to use `std::make_unique` instead of the `new/assignment` idiom to set up your `std::unique_ptr` instances. 

Granted, my brain keeps thinking in terms of the outmoded constructor+`new` rather than `std::make_unique` best practices, so this sub-par idiom may show up in a few places still, after today. Alas. I blame my advancing age. *(koff koff koff)*😇
