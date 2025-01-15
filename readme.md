# Dynamic Text
![DynamicText](https://github.com/user-attachments/assets/2405a379-0284-4ad5-bc19-9d74861c5e4c)

## Included Effects
- Text Wave (identifier: wave)
- Text Shake (identifier: shake)

## How to use
1. Place dynamic text script on TextMesh Pro object
2. Use this format to call effects `<[identifier]:[effect input]>` (example: `<shake:4.5>`)
3. Use this format to cancel effects `</[identifier]>` (If no cancel command is given, effects will continue to the end of the text)

## How to create custom effects
Creating a custom effect takes *3 steps*
1. Create a new class that derives from `TextEffect`
2. Implement the abstract function `UpdateText`
   - TextVertices is the array of vertices of each character's mesh that you will modify directly
     - Each Character's mesh has 4 vertices in a random order
     - **(Only modify the vertices of characters from `start (inclusive)` to `end (non-inclusive)`)**
   - A typical effect looks something like this:
    ```
    public override void UpdateText(Vector3[][] textVertices, int start, int end, float input) {
        for(int i = start; i < end; i++) {
           for(int e = 0; e < 4; e++) {
               textVertices[i][j] = *Do something to the vertex*
           }
        }
    }
    ```
3. In the text parser script, add an entry into the textRegex array using this format `new([identifier, new [class])`
   ![image](https://github.com/user-attachments/assets/bf879a58-568e-42b7-8f43-3c94125329bd)
