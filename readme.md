# Dynamic Text
![DynamicText](https://github.com/user-attachments/assets/d571eb06-d711-4bfe-bdd7-df399efab808)

## Included Effects
- Vertex Effects
   - Text Wave (identifier: wave)
   - Text Shake (identifier: shake)
- Color Effects
   - Color change (identifier: color)
- Animation Effects
   - Grow (identifier: grow)
   - Slide In (identifier: slide)
- Speed Effects
   - Pause (identifier: pause)
   - Speed Change (identifier: speed)


 
## How to use
1. Place dynamic text script on TextMesh Pro object
2. Use this format to call effects `<[identifier]:[effect input]>` (example: `<shake:4.5>`)
3. Use this format to cancel effects `</[identifier]>` (If no cancel command is given, effects will continue to the end of the text)

## How to create custom effects
Creating a custom effect takes *3 steps*
1. Create a new class that derives from either `TextAnimationEffect`, `TextVertexEffect`, or `TextColorEffect`
2. Implement the abstract function in your chosen parent class
   - `input` is usually used to affect the strength of the effect
   - TextVertexArray 
     - Each Character's mesh has 4 vertices in a random order
     - **(Only modify the vertices of characters from `start (inclusive)` to `end (non-inclusive)`)**'
     - A typical vertex effect looks something like this:
    ```
    public override void UpdateText(Vector3[][] textVertices, int start, int end, float input) {
        for(int i = start; i < end; i++) {
           for(int e = 0; e < 4; e++) {
               textVertices[i][j] = *Do something to the vertex*
           }
        }
    }
    ```
   - TextColorEffect
        - Each Character's mesh has 4 colors (one for each vertex) in a random order
        - **(Only modify the vertices of characters from `start (inclusive)` to `end (non-inclusive)`)**'
        - A typical color effect looks something like this:
       ```
       public override void UpdateText(Color32[][] textColors, int start, int end, float input) {
           for(int i = start; i < end; i++) {
              for(int e = 0; e < 4; e++) {
                  textColor[i][j] = *Do something to the color*
              }
           }
       }
       ```
    - TextAnimationEffect
        - Each Character's mesh has 4 vertices/colors in a random order
        - A typical animation effect looks something like this:
       ```
       public override void UpdateText(Vector3[] characterVertices, Color32[] characterColors, float input, float time) {
           for(int e = 0; e < 4; e++) {
               textVertices[i][j] = *Do something to the vertex based on the current time passed*
           }
       }
       ```
3. In the text parser script, add an entry into the textRegex array using this format `new(["identifier", new [class])`
   ![image](https://github.com/user-attachments/assets/bf879a58-568e-42b7-8f43-3c94125329bd)
