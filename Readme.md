# Repository for routing algorithms implemented in C#.

The following algorithms are currently implemented
* Dijkstra
* SSMTSPP from C. Bazgan, J. Kager, C. Thielen, and D. Vanderpooten, "A general label setting algorithm and tractability analysis for the multiobjective temporal shortest path problem" Networks. 85 (2025), 76–90. [doi.org/10.1002/net.22253](https://doi.org/10.1002/net.22253)

# GtfsPlanner

You can use the included `GtfsPlanner` project to experiment with the algorithms. When you build the project you can use the program to plan routes in GTFS networks. For example you can use the following command
```
./GtfsPlanner  --gtfs GTFS.zip --date 2025.03.24 --start "Start station" --dest "Destination station" LatestDepartureEarliestArrival
``` 
to calculate all latest departure - earliest arrival paths in the given network on the given day.

# License
Unless otherwise noted all code in this repository is distributed under the GNU General Public License version 3. See the included [License](License) file.