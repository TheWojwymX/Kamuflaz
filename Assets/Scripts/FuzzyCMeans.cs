using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyCMeans
{
    private int numberOfClusters;
    private float fuzziness;
    
    public FuzzyCMeans(int numberOfClusters, float fuzziness)
    {
        this.numberOfClusters = numberOfClusters;
        this.fuzziness = fuzziness;
    }

    public Color[] ClusterColors(Color[] colors)
    {
        int n = colors.Length;
        int m = numberOfClusters;

        // Initialize membership matrix and cluster centroids
        float[,] membership = new float[n, m];
        Color[] centroids = new Color[m];

        // Randomly initialize centroids
        for (int i = 0; i < m; i++)
        {
            centroids[i] = colors[UnityEngine.Random.Range(0, n)];
        }

        bool hasConverged = false;
        int iterations = 0;
        const int maxIterations = 30;

        while (!hasConverged && iterations < maxIterations)
        {
            // Step 1: Update membership matrix
            for (int i = 0; i < n; i++)
            {
                float sum = 0;

                for (int j = 0; j < m; j++)
                {
                    float distance = Vector3.Distance(new Vector3(colors[i].r, colors[i].g, colors[i].b),
                        new Vector3(centroids[j].r, centroids[j].g, centroids[j].b));
                    float d = 0;

                    // Calculate distance sum
                    for (int k = 0; k < m; k++)
                    {
                        float dK = Vector3.Distance(new Vector3(colors[i].r, colors[i].g, colors[i].b),
                            new Vector3(centroids[k].r, centroids[k].g, centroids[k].b));
                        if (dK > 0)
                            d += Mathf.Pow(distance / dK, 2 / (fuzziness - 1));
                    }

                    membership[i, j] = 1 / (d + 1e-6f); // Avoid division by zero
                    sum += membership[i, j]; // Collect sum for normalization
                }

                // Normalize membership values
                for (int j = 0; j < m; j++)
                {
                    membership[i, j] = membership[i, j] / sum; // Normalize to ensure membership sums to 1
                }
            }

            // Step 2: Update centroids
            Color[] newCentroids = new Color[m];

            for (int j = 0; j < m; j++)
            {
                float totalMembership = 0;

                // Initialize the centroid to zero
                newCentroids[j] = Color.black;

                for (int i = 0; i < n; i++)
                {
                    float membershipValue = Mathf.Pow(membership[i, j], fuzziness);
                    totalMembership += membershipValue;
                    newCentroids[j] += membershipValue * colors[i]; // Accumulate based on membership
                }

                if (totalMembership > 0)
                {
                    newCentroids[j] /= totalMembership; // Normalize the new centroid
                }
                else
                {
                    // Handle case where total membership is zero (optional)
                    newCentroids[j] = centroids[j]; // Keep the previous centroid
                }
            }

            // Check for convergence
            hasConverged = true;
            for (int j = 0; j < m; j++)
            {
                if (Vector3.Distance(new Vector3(centroids[j].r, centroids[j].g, centroids[j].b),
                        new Vector3(newCentroids[j].r, newCentroids[j].g, newCentroids[j].b)) > 0.001f)
                {
                    hasConverged = false;
                    break;
                }
            }

            centroids = newCentroids;
            iterations++;
        }

        if (iterations >= maxIterations)
        {
            Debug.LogWarning("Fuzzy C-Means did not converge within the maximum iterations.");
        }

        return centroids; // Return the clustered centroids (dominant colors)
    }
}

