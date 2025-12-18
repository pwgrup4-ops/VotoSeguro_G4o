
import { Injectable } from '@angular/core';
import { Storage, ref, uploadBytes, getDownloadURL } from '@angular/fire/storage';
import { Observable, from, switchMap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  constructor(private storage: Storage) { }

  /**
   * Sube un archivo a Firebase Storage y devuelve la URL de descarga.
   * @param file Archivo a subir (Foto o Logo).
   * @param path Ruta dentro de Firebase Storage (ej: 'candidates/photos/').
   * @param fileName Nombre del archivo.
   * @returns Observable<string> con la URL de descarga.
   */
  uploadFile(file: File, path: string, fileName: string): Observable<string> {
    const storageRef = ref(this.storage, `${path}${fileName}`);

    // 1. Subir el archivo (uploadBytes)
    const uploadTask = from(uploadBytes(storageRef, file));

    // 2. Esperar la finalización de la subida y obtener la URL de descarga (getDownloadURL)
    return uploadTask.pipe(
      switchMap(() => from(getDownloadURL(storageRef)))
    );
  }
}
