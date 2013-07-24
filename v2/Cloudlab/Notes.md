This instance function determines whether an entity at the provided path is a file[^1].

### Usage

    b = fs.isFile(path)
    
| Parameter | Required | Type | Description |
| - | - | - | - |
| **path** | Yes | `String` | The path to the file system entity that you wish to determine whether or not it is a file |
| **b** | &mdash; | `Boolean` | Whether or not the provided path is a file |

<div class="alert alert-block">
<h4 style="text-align:center">Warning</h4>
If the path provided to this function does not correspond to an entry in the file system (whether a file or directory), a `FileError` exception will be thrown. To guard against this possible type of error, use the `exists()` function that can be used to determine whether or not the path is valid.
</div>

[^1]: This function is conformant to the [CommonJS Filesystem/A/0](http://wiki.commonjs.org/wiki/Filesystem/A/0#Tests "External Link") proposal